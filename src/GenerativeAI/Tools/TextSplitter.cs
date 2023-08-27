using Automation.GenerativeAI.Interfaces;
using System;
using System.Collections.Generic;

namespace Automation.GenerativeAI.Tools
{
    /// <summary>
    /// Splits the given text into smaller chunks
    /// </summary>
    public class TextSplitter
    {
        /// <summary>
        /// Max number of characters in the chunk to be used by text splitter
        /// </summary>
        protected int chunkSize;

        /// <summary>
        /// Max number of character overlap between the two consecutive chunks.
        /// </summary>
        protected int chunkOverlap;
        private char[] separators = { ' ', '\t', '\n', '\u2029', '\u2028'};

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="chunkSize"></param>
        /// <param name="chunkOverlap"></param>
        protected TextSplitter(int chunkSize, int chunkOverlap) 
        {
            this.chunkSize = chunkSize;
            this.chunkOverlap = chunkOverlap;
        }

        /// <summary>
        /// Creates a text splitter with parameters
        /// </summary>
        /// <param name="chunkSize">Max character length for each chunk</param>
        /// <param name="chunkOverlap">Size of chunk overlap</param>
        /// <returns>TextSplitter object</returns>
        public static TextSplitter WithParameters(int chunkSize = 500, int chunkOverlap = 20)
        {
            return new TextSplitter(chunkSize, chunkOverlap);
        }

        /// <summary>
        /// Splits the given text based on tokens/separators.
        /// </summary>
        /// <param name="text">Input text</param>
        /// <returns>List of tokens</returns>
        protected virtual IEnumerable<string> SplitOnTokens(string text)
        {
            return text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Splits the given text object into smaller chunks.
        /// </summary>
        /// <param name="text">TextObject</param>
        /// <returns>List of splitted text objects</returns>
        public IEnumerable<ITextObject> Split(ITextObject text)
        {
            if (string.IsNullOrEmpty(text.Text)) yield return text;

            if(text.Text.Length <= chunkSize) yield return text;

            var tokens = SplitOnTokens(text.Text);

            int chunklen = 0;
            int chunkCount = 0;
            List<string> chunks = new List<string>();
            foreach (var token in tokens)
            {
                chunklen += token.Length + 1;
                if (chunklen > chunkSize) 
                {
                    var name = $"{text.Name}, Chunk: {chunkCount++}";
                    var obj = TextObject.Create(name, string.Join(" ", chunks), text.Class);
                    List<string> overlaps = new List<string>();
                    chunklen = 0;
                    for(int i = chunks.Count-1; i >= 0; i--)
                    {
                        if (chunklen + chunks[i].Length > chunkOverlap) break;
                        
                        chunklen += chunks[i].Length + 1;
                        overlaps.Add(chunks[i]);
                    }
                    overlaps.Reverse();
                    chunks = overlaps;
                    chunklen += token.Length;
                    yield return obj;
                }
                chunks.Add(token);
            }
        }
    }
}
