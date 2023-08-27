using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Automation.GenerativeAI.Chat
{
    /// <summary>
    /// Represents Prompt Template
    /// </summary>
    public class PromptTemplate
    {
        private readonly string pattern = @"\{\{\s*\$([\w\.]+)\s*\}\}";
        private readonly string replacepattern = @"\{\{\s*\$[\w\.]+\s*\}\}";

        /// <summary>
        /// Default constructor that takes template string. The variable input
        /// must be represented by {{$input}} text in the template, where
        /// input is the variable value in the template.
        /// </summary>
        /// <param name="template">Template string</param>
        /// <param name="role">Role of the author of this prompt, the default value is user. 
        /// The role could also be system or assistant.</param>
        public PromptTemplate(string template, Role role = Role.user) 
        {
            Role = role;
            Variables = new List<string>();
            Template = template;
            ParseVariables(template);
        }

        /// <summary>
        /// Creates the prompt template with a template file.
        /// </summary>
        /// <param name="templatefile">Full path of the template file.</param>
        /// <param name="role">Role of the author of this prompt.</param>
        /// <returns>PromptTemplate</returns>
        public static PromptTemplate WithTemplateFile(string templatefile, Role role = Role.user)
        {
            if (!File.Exists(templatefile)) return null;
            
            var template = File.ReadAllText(templatefile);
            return new PromptTemplate(template, role);
        }

        /// <summary>
        /// Gets the original template string
        /// </summary>
        public string Template { get; private set; }

        /// <summary>
        /// Gets the list of variables in the template
        /// </summary>
        public List<string> Variables { get; private set; }

        /// <summary>
        /// Reole of the author of the message created by this prompt, the default value is 'user'
        /// </summary>
        public Role Role { get; private set; }

        /// <summary>
        /// Formats the template to the ChatMessage using the context dictionary.
        /// </summary>
        /// <param name="context">Dictionary containing variables and corresponding values.</param>
        /// <returns>A ChatMessage object</returns>
        public ChatMessage FormatMessage(ExecutionContext context)
        {
            Regex r = new Regex(replacepattern);
            Regex r2 = new Regex(@"\$([\w\.]+)");
            var matches = r.Matches(Template);
            string message = Template;
            foreach (Match match in matches)
            {
                var token = match.Value;

                var m = r2.Match(token);
                var variable = m.Groups[1].Value;

                object value = string.Empty;
                if(context.TryGetValue(variable, out value))
                {
                    message = message.Replace(token, (string)value);
                }
                else
                {
                    Logger.WriteLog(LogLevel.Warning, LogOps.Result, $"Couldn't find value for variable: '{variable}' in the context.");
                }
            }

            return new ChatMessage(Role, message); 
        }

        /// <summary>
        /// Preses the given template string to extract variables and add them to the Variables property.
        /// </summary>
        /// <param name="template">Template string</param>
        private void ParseVariables(string template) 
        {
            var variables = new List<string>();
            Regex r = new Regex(pattern);
            var matches = r.Matches(template);
            foreach (Match match in matches )
            {
                var variable = match.Groups[1].Value.Trim();
                variables.Add(variable);
            }

            Variables.AddRange(variables.Distinct());
        }
    }
}
