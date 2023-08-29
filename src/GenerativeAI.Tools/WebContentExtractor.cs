using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Automation.GenerativeAI.Tools
{
    /// <summary>
    /// Implements web content extractor utility using HtmlAgilityPack
    /// </summary>
    public static class WebContentExtractor
    {
        /// <summary>
        /// Extracts text content from a webpage.
        /// </summary>
        /// <param name="url">URL of the webpage</param>
        /// <returns>The plain text version of the HTML content.</returns>
        public static string GetTextContentFromWebpage(string url)
        {
            var htmlweb = new HtmlWeb();

            var doc = htmlweb.Load(url);

            //search the root content
            var nodes = doc.DocumentNode.SelectNodes(@"//div[@id='mw-content-text']");
            if (nodes == null || nodes.Count == 0)
            {
                nodes = new HtmlNodeCollection(doc.DocumentNode)
                {
                    doc.DocumentNode
                };
            }

            using (var sw = new StringWriter())
            {
                foreach (var node in nodes)
                {
                    // Convert the HTML string to plain text
                    ConvertTo(node: node,
                              outText: sw,
                              counters: new Dictionary<HtmlNode, int>());
                }

                sw.Flush();
                return sw.ToString();
            }
        }

        /// <summary>
        /// Extracts or parses the text content from a given html content.
        /// </summary>
        /// <param name="html">A string containing HTML document.</param>
        /// <returns>The plain text version of the HTML content.</returns>
        public static string GetTextFromHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            
            //search the root content
            var nodes = doc.DocumentNode.SelectNodes(@"//div[@id='mw-content-text']");
            if (nodes == null || nodes.Count == 0)
            {
                nodes = new HtmlNodeCollection(doc.DocumentNode)
                {
                    doc.DocumentNode
                };
            }

            using (var sw = new StringWriter())
            {
                foreach(var node in nodes)
                {
                    // Convert the HTML string to plain text
                    ConvertTo(node: node,
                              outText: sw,
                              counters: new Dictionary<HtmlNode, int>());
                }
                
                sw.Flush();
                return sw.ToString();
            }
        }

        #region Method: GetContentFromHtmlFile (public - static)
        /// <summary>
        /// Converts the HTML content from a given file path to plain text.
        /// </summary>
        /// <param name="path">The path to the HTML file.</param>
        /// <returns>The plain text version of the HTML content.</returns>
        public static string GetContentFromHtmlFile(string path)
        {
            var doc = new HtmlDocument();

            // Load the HTML file
            doc.Load(path);

            using (var sw = new StringWriter())
            {
                // Convert the HTML document to plain text
                ConvertTo(node: doc.DocumentNode,
                          outText: sw,
                          counters: new Dictionary<HtmlNode, int>());
                sw.Flush();
                return sw.ToString();
            }
        }
        #endregion

        #region Method: ConvertTo (static)
        /// <summary>
        /// Helper method to convert each child node of the given node to text.
        /// </summary>
        /// <param name="node">The HTML node to convert.</param>
        /// <param name="outText">The writer to output the text to.</param>
        /// <param name="counters">Keep track of the ol/li counters during conversion</param>
        private static void ConvertContentTo(HtmlNode node, TextWriter outText, Dictionary<HtmlNode, int> counters)
        {
            // Convert each child node to text
            foreach (var subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText, counters);
            }
        }
        #endregion

        #region Method: ConvertTo (private - static)
        /// <summary>
        /// Converts the given HTML node to plain text.
        /// </summary>
        /// <param name="node">The HTML node to convert.</param>
        /// <param name="outText">The writer to output the text to.</param>
        /// <param name="counters"></param>
        private static void ConvertTo(HtmlNode node, TextWriter outText, Dictionary<HtmlNode, int> counters)
        {
            string html;

            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // Don't output comments
                    break;
                case HtmlNodeType.Document:
                    // Convert entire content of document node to text
                    ConvertContentTo(node, outText, counters);
                    break;
                case HtmlNodeType.Text:
                    // Ignore script and style nodes
                    var parentName = node.ParentNode.Name;
                    if ((parentName == "script") || (parentName == "style"))
                    {
                        break;
                    }

                    // Get text from the text node
                    html = ((HtmlTextNode)node).Text;

                    // Ignore special closing nodes output as text
                    if (HtmlNode.IsOverlappedClosingElement(html) || string.IsNullOrWhiteSpace(html))
                    {
                        break;
                    }

                    // Write meaningful text (not just white-spaces) to the output
                    outText.Write(HtmlEntity.DeEntitize(html));
                    break;
                case HtmlNodeType.Element:
                    switch (node.Name.ToLowerInvariant())
                    {
                        case "p":
                        case "div":
                        case "br":
                        case "table":
                            // Treat paragraphs and divs as new lines
                            outText.Write("\n");
                            break;
                        case "li":
                            // Treat list items as dash-prefixed lines
                            if (node.ParentNode.Name == "ol")
                            {
                                if (!counters.ContainsKey(node.ParentNode))
                                {
                                    counters[node.ParentNode] = 0;
                                }
                                counters[node.ParentNode]++;
                                outText.Write("\n" + counters[node.ParentNode] + ". ");
                            }
                            else
                            {
                                outText.Write("\n- ");
                            }
                            break;
                        case "a":
                            // convert hyperlinks to include the URL in parenthesis
                            if (node.HasChildNodes)
                            {
                                ConvertContentTo(node, outText, counters);
                            }
                            if (node.Attributes["href"] != null)
                            {
                                outText.Write($" ({node.Attributes["href"].Value})");
                            }
                            break;
                        case "th":
                        case "td":
                            outText.Write(" | ");
                            break;
                    }

                    // Convert child nodes to text if they exist (ignore a href children as they are already handled)
                    if (node.Name.ToLowerInvariant() != "a" && node.HasChildNodes)
                    {
                        ConvertContentTo(node: node,
                                         outText: outText,
                                         counters: counters);
                    }
                    break;
            }
        }
        #endregion

    }
}
