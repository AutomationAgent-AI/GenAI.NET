using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automation.GenerativeAI.Interfaces
{
    /// <summary>
    /// Represents a text object
    /// </summary>
    public class TextObject : ITextObject
    {
        /// <summary>
        /// Default class name for the text object classification
        /// </summary>
        public static readonly string DefaultClass = "Unclassified";

        /// <summary>
        /// Creates Text Object
        /// </summary>
        /// <param name="name">Name of the object, something like document name</param>
        /// <param name="text">The text content</param>
        /// <param name="classname">The classification of the text object</param>
        /// <returns>ITextObject</returns>
        public static ITextObject Create(string name, string text, string classname = "")
        {
            return new TextObject() { Name = name, Text = text, Class = string.IsNullOrEmpty(classname) ? DefaultClass : classname };
        }

        /// <summary>
        /// Compares two string to provide match score using fuzzy logic
        /// </summary>
        /// <param name="s1">First string</param>
        /// <param name="s2">Second string</param>
        /// <returns>Match score between 0 and 1</returns>
        public static double FuzzyMatch(string s1, string s2)
        {
            int n = s1.Length;
            int m = s2.Length;
            int[,] distanceMatrix = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return 0; //no match
            }

            if (m == 0)
            {
                return 0; //no match
            }

            // Step 2
            for (int i = 0; i <= n; distanceMatrix[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; distanceMatrix[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (s2[j - 1] == s1[i - 1]) ? 0 : 1;

                    // Step 6
                    distanceMatrix[i, j] = Math.Min(
                        Math.Min(distanceMatrix[i - 1, j] + 1, distanceMatrix[i, j - 1] + 1),
                        distanceMatrix[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            double len = Math.Max(m, n);
            var percent = 1 - distanceMatrix[n, m] / len;

            return percent;
        }

        /// <summary>
        /// Gets the longest common substring between the two given strings
        /// </summary>
        /// <param name="s1">First string</param>
        /// <param name="s2">Second string</param>
        /// <returns>Longest common string</returns>
        public static string LongestCommonSubstring(string s1, string s2)
        {
            int m = s1.Length, n = s2.Length;
            int[,] lcs = new int[m + 1, n + 1];
            int maxlen = 0;
            int endingindex = m;

            for (int i = 0; i <= m; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    if (i == 0 || j == 0)
                        lcs[i, j] = 0;
                    else if (s1[i - 1] == s2[j - 1])
                    {
                        lcs[i, j] = lcs[i - 1, j - 1] + 1;
                        if (lcs[i, j] > maxlen)
                        {
                            maxlen = lcs[i, j];
                            endingindex = i;
                        }
                    }
                    else
                        lcs[i, j] = 0;
                }
            }

            return s1.Substring(endingindex - maxlen, maxlen);
        }

        /// <summary>
        /// Name or source of the text object, something like document name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The classification of the text object
        /// </summary>
        public string Class { get; private set; }

        /// <summary>
        /// The text content 
        /// </summary>
        public string Text { get; private set; }
    }
}
