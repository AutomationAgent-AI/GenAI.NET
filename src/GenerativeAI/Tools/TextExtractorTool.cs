using Automation.GenerativeAI.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.Tools
{
    /// <summary>
    /// TextExtractTool that extracts text from a given one or more source text file or pdf from a directory.
    /// </summary>
    public class TextExtractorTool : FunctionTool
    {
        private ParameterDescriptor parameter;

        /// <summary>
        /// Constructor for TextExtractorTool
        /// </summary>
        private TextExtractorTool()
        {
            Name = "TextExtractor";
            Description = "Extracts text from a given one or more source text file or pdf from a directory";
            parameter = new ParameterDescriptor()
            {
                Name = "source",
                Type = TypeDescriptor.StringType,
                Description = "Full path of the source file or folder from where text needs to be extracted"
            };
        }

        /// <summary>
        /// Creates a TextExtractorTool
        /// </summary>
        /// <returns>TextExtractorTool</returns>
        public static TextExtractorTool Create()
        {
            return new TextExtractorTool();
        }

        internal static List<ITextObject> ExtractTextObjects(string source)
        {
            var txtservice = Application.GetTextProviderService();
            if (txtservice == null) throw new System.Exception("TextProviderService not found!!");

            var extensions = new[] { ".txt", ".csv", ".pdf" };
            var files = Application.GetFiles(source.ToString(), extensions);

            List<ITextObject> textObjects = new List<ITextObject>();
            foreach (var file in files)
            {
                var texts = txtservice.EnumerateText(file, "English");
                textObjects.AddRange(texts.ToList());
            }

            return textObjects;
        }

        /// <summary>
        /// Extracts text from a given source. The source can be a full path of a file
        /// or a folder containing txt or pdf files.
        /// </summary>
        /// <param name="source">Full path to get a list of files.</param>
        /// <returns>Text content from all the files from source.</returns>
        /// <exception cref="System.Exception"></exception>
        public static string ExtractText(string source)
        {
            var textObjects = ExtractTextObjects(source);

            return string.Join("\n\n", textObjects.Select(x => x.Text));
        }

        /// <summary>
        /// Executes the text extractor tool to extract text based on the execution context
        /// </summary>
        /// <param name="context">ExecutionContext</param>
        /// <param name="output">Extracted text content</param>
        /// <returns>True if successful</returns>
        protected bool Execute(ExecutionContext context, out string output)
        {
            object source;
            output = string.Empty;

            if(!context.TryGetValue(parameter.Name, out source))
                return false;

            output = ExtractText(source.ToString());
            return true;
        }

        /// <summary>
        /// Provides function descriptor
        /// </summary>
        /// <returns>FunctionDescriptor</returns>
        protected override FunctionDescriptor GetDescriptor()
        {
            var function = new FunctionDescriptor(Name, Description, new List<ParameterDescriptor> { parameter });
            
            return function;
        }

        /// <summary>
        /// Overrides the core execution logic
        /// </summary>
        /// <param name="context">ExecutionContext</param>
        /// <returns>Execution Result</returns>
        protected override async Task<Result> ExecuteCoreAsync(ExecutionContext context)
        {
            string output = string.Empty;
            var result = new Result();
            result.success = await Task.Run(() => Execute(context, out output));
            result.output = output;
            return result;
        }
    }
}
