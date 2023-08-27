using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Automation.GenerativeAI.Services
{
    /// <summary>
    /// Implements an interface to provide ITexObject from specific file type
    /// </summary>
    internal interface ITextObjectProvider
    {
        /// <summary>
        /// Get the list of supported file extensions.
        /// </summary>
        IEnumerable<string> SupportedExtensions { get; }

        /// <summary>
        /// Creates a list of text providers for specific file to extract the text from each data item (pages or rows).
        /// </summary>
        /// <param name="filepath">Input file path</param>
        /// <param name="language">Language of the content</param>
        /// <returns>List of ITextProvider</returns>
        IEnumerable<ITextObject> GetTexts(string filepath, string language);
    }

    class TextProviderService
    {
        private List<ITextObjectProvider> providers = new List<ITextObjectProvider>();

        public TextProviderService()
        {
            providers.Add(new TextFileTextProvider());
            providers.Add(new CSVTextProvider());
            providers.Add(new PDFTextProvider());
        }

        public IEnumerable<ITextObject> EnumerateText(string source, string language)
        {
            IEnumerable<string> files = Enumerable.Empty<string>();
            bool extractpages = false;
            if (source.Contains("*"))
            {
                var searchpattern = Path.GetFileName(source);
                var directory = Path.GetDirectoryName(source);
                files = Directory.GetFiles(directory, searchpattern, SearchOption.AllDirectories);

                if (!files.Any()) yield return TextObject.Create("", source);
            }
            else if (Directory.Exists(source))
            {
                files = GetAllFilesFromFolder(source);
            }
            else if (File.Exists(source))
            {
                extractpages = true;
                files = Enumerable.Repeat(source, 1);
            }
            else
            {
                yield return TextObject.Create("", source);
            }

            foreach (var item in files)
            {
                IEnumerable<ITextObject> texts = Enumerable.Empty<ITextObject>();
                var extension = Path.GetExtension(item).Replace(".", "").ToLower();
                var name = Path.GetFileName(item);
                var provider = providers.Where(p => p.SupportedExtensions.Contains(extension)).FirstOrDefault();
                if (provider == null)
                {
                    Logger.WriteLog(LogLevel.Info, LogOps.NotFound, string.Format("No text provider for file: {0}", name));
                    texts = Enumerable.Repeat(TextObject.Create(name, string.Empty), 1);
                }
                else
                {
                    texts = provider.GetTexts(item, language);
                }
                if (extractpages)
                {
                    foreach (var txt in texts)
                    {
                        yield return txt;
                    }
                }
                else
                {
                    yield return TextObject.Create(name, string.Join("\n", texts.Select(x => x.Text).ToArray()), texts.FirstOrDefault().Class);
                }
            }
        }

        private IEnumerable<string> GetAllFilesFromFolder(string source)
        {
            IEnumerable<string> files = Directory.GetFiles(source);
            var folders = Directory.GetDirectories(source);
            foreach (var item in folders)
            {
                files = files.Concat(GetAllFilesFromFolder(item));
            }
            return files;
        }

        public ITextObject GetAllText(string source, string language)
        {
            var texts = EnumerateText(source, language).ToArray();
            if (!texts.Any()) return TextObject.Create(string.Empty, string.Empty);

            return TextObject.Create(texts.FirstOrDefault().Name, string.Join("\n", texts.Select(x => x.Text).ToArray()));
        }

        public void RegisterTextObjectProvider(ITextObjectProvider textObjectProvider)
        {
            providers.Add(textObjectProvider);
        }
    }

    class TextFileTextProvider : ITextObjectProvider
    {
        public IEnumerable<string> SupportedExtensions { get { return new[] { "txt", "log", "text" }; } }

        public IEnumerable<ITextObject> GetTexts(string filepath, string language)
        {
            var fileinfo = new FileInfo(filepath);
            ITextObject text = TextObject.Create(fileinfo.Name, File.ReadAllText(filepath), fileinfo.Directory.Name);
            return Enumerable.Repeat(text, 1);
        }
    }

    class IdentityTextProvider : ITextObjectProvider
    {
        public IEnumerable<string> SupportedExtensions { get { return Enumerable.Empty<string>(); } }

        public IEnumerable<ITextObject> GetTexts(string content, string language)
        {
            ITextObject text = TextObject.Create("", content);
            return Enumerable.Repeat(text, 1);
        }
    }

    class CSVTextProvider : ITextObjectProvider
    {
        public IEnumerable<string> SupportedExtensions { get { return Enumerable.Repeat("csv", 1); } }

        public IEnumerable<ITextObject> GetTexts(string filepath, string language)
        {
            var list = new List<ITextObject>();
            var name = Path.GetFileNameWithoutExtension(filepath);
            using (var reader = new StreamReader(filepath))
            {
                int row = -1;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    var classname = values.Length > 1 ? values[1] : "";
                    row++;
                    if (row < 1) continue;
                    list.Add(TextObject.Create(string.Format("{0} - Row {1}", name, row), line, classname));
                }
            }
            return list;
        }
    }
}
