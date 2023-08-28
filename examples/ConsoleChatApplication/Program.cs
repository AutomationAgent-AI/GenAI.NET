using Automation.GenerativeAI;
using Automation.GenerativeAI.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleChatApplication
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Create GenAI service
            var service = Application.GetAIService();

            var apikey = ""; //Pass your API key, or set your api key to OPENAI_API_KEY environment variable

            //Create Language Model
            var llm = service.CreateOpenAIModel("gpt-3.5-turbo-0613", apikey);

            var chatid = Guid.NewGuid().ToString();

            //Create Conversation
            var chat = service.CreateConversation(chatid, llm);

            Console.WriteLine("Welcome to the Console Chat Applicaiton!!, This application allows you chat with the smart conversation agent.");
            Console.WriteLine("You can use following commands to interact with the application meaningfully");
            Console.WriteLine("/quit: To quit the application.");
            Console.WriteLine("/restart: To restart a new conversation");
            Console.WriteLine("/system: To pass a system message to the agent");
            Console.WriteLine("/addtool: To add a new tool to the conversation");
            Console.WriteLine("/adddoc: To add a new document for the context to the conversation");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("AI: Hi there, How may I assist you?");
            Console.Write("You: ");

            while (true)
            {
                var msg = Console.ReadLine();
                string path = string.Empty;
                bool system = false;
                if (msg.StartsWith("/"))
                {
                    var cmd = msg.Split(':');

                    switch (cmd[0].ToLower())
                    {
                        case "/quit":
                            return; //Exit from application.
                        case "/restart":
                            chatid = Guid.NewGuid().ToString();
                            chat = service.CreateConversation(chatid, llm);
                            break;
                        case "/system":
                            Console.Write("System:");
                            system = true;
                            break;
                        case "/addtool":
                            Console.WriteLine("AI: Please provide the full path of .NET DLL to add as a tool.");
                            Console.Write("Path: ");
                            path =Console.ReadLine();
                            if (File.Exists(path))
                            {
                                var tool = new DLLFunctionTools(path);
                                chat.AddToolSet(tool);
                            }
                            else
                            {
                                Console.WriteLine($"AI: Could not find {path}, How may I assist you?");
                                Console.Write("You: ");
                            }
                            break;
                        case "/adddoc":
                            Console.WriteLine("AI: Please provide the full path of a document to add context.");
                            Console.Write("Path: ");
                            path = Console.ReadLine();
                            bool error = true;
                            if (File.Exists(path))
                            {
                                var vdbpath = Path.ChangeExtension(path, ".vdb");
                                IVectorStore store = null;
                                if (!File.Exists(vdbpath))
                                {
                                    store = Application.CreateVectorDatabaseForSemanticSearch(path, vdbpath);
                                }
                                else
                                {
                                    store = service.DeserializeVectorStore(vdbpath);
                                }

                                if (File.Exists(vdbpath))
                                {
                                    Console.WriteLine($"AI: document {Path.GetFileName(path)} has been add to knowledge base, How may I assit you?");
                                    Console.Write("You: ");
                                    chat.AddContext(store);
                                    error = false;
                                }
                            }
                            if(error)
                            {
                                Console.WriteLine($"AI: Unable to add document at {path}, How may I assist you?");
                                Console.Write("You: ");
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    var role = system ? Role.system : Role.user;
                    var chatmsg = new ChatMessage(role, msg);
                    chat.AppendMessage(chatmsg);
                    var response = await chat.GetResponseAsync(0.8);
                    Console.WriteLine($"AI: {response.content}");
                    Console.Write("You: ");
                }
            }
        }
    }
}
