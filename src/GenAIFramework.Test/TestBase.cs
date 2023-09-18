using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Utilities;
using System;
using System.IO;
using System.Reflection;

namespace GenAIFramework.Test
{
    public abstract class TestBase
    {
        protected string RootPath = string.Empty;
        private ILanguageModel languageModel;

        internal static string GetDLLPath()
        {
            var asm = Assembly.GetExecutingAssembly();
            var codebase = asm.CodeBase;
            UriBuilder uri = new UriBuilder(codebase);
            string path = Uri.UnescapeDataString(uri.Path);

            return path;
        }

        public TestBase(string name)
        {
            RootPath = Assembly.GetExecutingAssembly().Location;
            var logfile = Path.Combine(RootPath, $@"..\..\..\..\..\tests\output\{name}.log");
            Logger.SetLogFile(logfile);
        }

        protected ILanguageModel LanguageModel
        {
            get
            {
                if (null == languageModel)
                {
                    languageModel = CreateLanguageModel();
                }

                return languageModel;
            }
        }

        protected abstract ILanguageModel CreateLanguageModel();
    }
}
