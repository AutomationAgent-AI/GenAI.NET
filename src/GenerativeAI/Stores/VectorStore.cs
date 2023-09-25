using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Utilities;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace Automation.GenerativeAI.Stores
{
    class MatchedObject : IMatchedObject
    {
        public double Score { get; set; }
        public IDictionary<string, string> Attributes { get; set; }
    }

    [Serializable]
    internal class VectorStore : IVectorStore
    {
        private ConcurrentBag<double[]> vectors = new ConcurrentBag<double[]>();
        //private List<double[]> vectors = new List<double[]>();
        //private List<IDictionary<string, string>> attributes = new List<IDictionary<string, string>>();
        private ConcurrentBag<IDictionary<string,string>> attributes = new ConcurrentBag<IDictionary<string, string>>();

        private IVectorTransformer transformer = null;
        private readonly string v1header = "Automation.Classifier.VectorStore v1.0";
        private VectorStore() { }

        /// <summary>
        /// Vector Store constructor
        /// </summary>
        /// <param name="transformer">Vector transformer</param>
        public VectorStore(IVectorTransformer transformer)
        {
            this.transformer = transformer;
        }

        public int VectorLength => transformer.VectorLength;

        public void Add(double[] vector, IDictionary<string, string> attributes)
        {
            vectors.Add(vector);
            this.attributes.Add(attributes);
        }

        private static IDictionary<string, string> ToDictionary(ITextObject text, bool savetext, int index)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("Name", text.Name);
            dict.Add("Class", text.Class);
            dict.Add("Index", index.ToString());
            if (savetext) { dict.Add("Text", text.Text); }
            return dict;
        }

        public void Add(IEnumerable<ITextObject> textObjects, bool savetext)
        {
            var validtxts = textObjects.Where(x => !string.IsNullOrEmpty(x.Text));
            int i = 0;
            Parallel.ForEach(validtxts, txt =>
            {
                var vec = transformer.Transform(txt.Text);
                vectors.Add(vec);
                Interlocked.Increment(ref i);
                var dict = ToDictionary(txt, savetext, i);
                attributes.Add(dict);
            });
        }

        public IEnumerable<IMatchedObject> Search(double[] vector, int resultcount)
        {
            if(resultcount > vectors.Count)
            {
                resultcount = vectors.Count;
            }

            ConcurrentBag<IMatchedObject> results = new ConcurrentBag<IMatchedObject>();

            Parallel.For(0, vectors.Count, idx =>
            {
                var match = new MatchedObject() { Attributes = attributes.ElementAt(idx) };
                var vec = vectors.ElementAt(idx);
                match.Score = 1 - vec.CosineDistance(vector);
                results.Add(match);
            });

            return results.OrderByDescending(t => t.Score).Take(resultcount);
        }

        public IEnumerable<IMatchedObject> Search(ITextObject textObject, int resultcount)
        {
            return Search(transformer.Transform(textObject.Text), resultcount);
        }

        public static VectorStore Create(string recepiefile)
        {
            var ext = Path.GetExtension(recepiefile);
            if (ext.ToLower().Contains("vdb"))
            {
                recepiefile = Path.ChangeExtension(recepiefile, "vdb");
            }

            var store = new VectorStore();
            using (var stream = new FileStream(recepiefile, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                var header = formatter.Deserialize(stream).To<string>();
                if (string.Compare(header, store.v1header) != 0)
                {
                    var error = "Invalid Vector Store model file format, header info is missing";
                    Logger.WriteLog(LogLevel.Error, LogOps.Result, error);
                    throw new FormatException(error);
                }

                using (var gzip = new GZipStream(stream, CompressionMode.Decompress, true))
                {
                    int nVectors = (int)formatter.Deserialize(gzip);
                    for (int i = 0; i < nVectors; ++i)
                    {
                        var veclen = (int)formatter.Deserialize(gzip);
                        var vector = new double[veclen];
                        for(int j = 0; j < veclen; ++j)
                        {
                            vector[j] = (double)formatter.Deserialize(gzip);
                        }
                        store.vectors.Add(vector);
                    }
                    int nAttributes = (int)formatter.Deserialize(gzip);
                    for (int k = 0; k < nAttributes; k++)
                    {
                        int n = (int)formatter.Deserialize(gzip);
                        var attribute = new Dictionary<string, string>();
                        for(int j = 0; j < n; ++j)
                        {
                            string key = (string)formatter.Deserialize(gzip);
                            string value = (string)formatter.Deserialize(gzip);
                            attribute.Add(key, value);
                        }
                        store.attributes.Add(attribute);
                    }

                    store.transformer = (IVectorTransformer)formatter.Deserialize(gzip);
                }
            }

            return store;
        }

        public void Save(string filepath)
        {
            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                //Write the header info
                formatter.Serialize(stream, v1header);

                using (var gzip = new GZipStream(stream, CompressionMode.Compress, true))
                {
                    //Serialize Vectors
                    formatter.Serialize(gzip, this.vectors.Count);
                    foreach (var item in vectors)
                    {
                        formatter.Serialize(gzip, item.Length);
                        for(int i = 0; i < item.Length; i++)
                        {
                            formatter.Serialize(gzip, item[i]);
                        }
                    }

                    //Serialize attributes
                    formatter.Serialize(gzip, attributes.Count);
                    foreach (var att in attributes)
                    {
                        formatter.Serialize(gzip, att.Count);
                        foreach (var a in att)
                        {
                            formatter.Serialize(gzip, a.Key);
                            formatter.Serialize(gzip, a.Value);
                        }
                    }

                    //Serialize vector transformer
                    //TODO: what if the transformer is not serializable
                    formatter.Serialize(gzip, transformer);
                }
            }
        }
    }
}
