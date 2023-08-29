﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.Interfaces
{
    /// <summary>
    /// Defines an implementation of a large language model
    /// </summary>
    public interface ILanguageModel
    {
        /// <summary>
        /// Name of the model
        /// </summary>
        string ModelName { get; }

        /// <summary>
        /// Gets response based on given history of messages.
        /// </summary>
        /// <param name="messages">A list of messages as a history. The Response is generated for  
        /// the last message using the history of messages as context.</param>
        /// <param name="temperature">A value between 0 to 1, that controls randomness of the response. 
        /// Higher temperature will lead to more randomness. Lower temperature will be more deterministic.</param>
        /// <returns>An LLMResponse response object</returns>
        Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, double temperature);

        /// <summary>
        /// If the language model supports function calling then this method can be called to
        /// get the response asynchronously based on the given history of messages. 
        /// </summary>
        /// <param name="messages">A list of messages as a history. The response is generated for 
        /// the last message using the history of messages as context.</param>
        /// <param name="functions">A list of function descriptors to match if the request resolves 
        /// to function calling.</param>
        /// <param name="temperature">A value between 0 to 1, that controls randomness of the response. 
        /// Higher temperature will lead to more randomness. Lower temperature will be more deterministic.</param>
        /// <returns>An LLMResponse response object</returns>
        Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, IEnumerable<FunctionDescriptor> functions, double temperature);
    }

    /// <summary>
    /// Transforms a given string to a vector
    /// </summary>
    public interface IVectorTransformer
    {
        /// <summary>
        /// Transforms the given TextObject to a vector
        /// </summary>
        /// <param name="textObject">Input text</param>
        /// <returns>Vector representation of the input textObject</returns>
        double[] Transform(string textObject);

        /// <summary>
        /// Learns and transforms a list of text objects into vector representation.
        /// </summary>
        /// <param name="textObjects">A list of text objects</param>
        /// <returns>list of vector</returns>
        double[][] Transform(IEnumerable<string> textObjects);

        /// <summary>
        /// Returns default length of vector generated by this transformer. 
        /// Returns zero if transformer is not yet initialized.
        /// </summary>
        int VectorLength { get; }
    }

    /// <summary>
    /// Vector match object
    /// </summary>
    public interface IMatchedObject
    {
        /// <summary>
        /// Matching score
        /// </summary>
        double Score { get; }

        /// <summary>
        /// Attributes of the matched object
        /// </summary>
        IDictionary<string, string> Attributes { get; }
    }

    /// <summary>
    /// Implements an interface to provide text content
    /// </summary>
    public interface ITextObject
    {
        /// <summary>
        /// Name of the object
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Class name of the text object
        /// </summary>
        string Class { get; }

        /// <summary>
        /// Text content
        /// </summary>
        string Text { get; }
    }

    /// <summary>
    /// Represents a vector store to store embedding vectors along with its attributes.
    /// </summary>
    public interface IVectorStore
    {
        /// <summary>
        /// Vectorizes and adds a given textObject to the store.
        /// </summary>
        /// <param name="textObjects">List of Input text objects to add to the store</param>
        /// <param name="savetext">Flag whether to save text as attribute in the store</param>
        void Add(IEnumerable<ITextObject> textObjects, bool savetext);

        /// <summary>
        /// Method to add a vector and its attributes to the database.
        /// </summary>
        /// <param name="vector">Input vector, must be of the same length as VectorLength property of this object</param>
        /// <param name="attributes">Attributes for the vector</param>
        void Add(double[] vector, IDictionary<string, string> attributes);

        /// <summary>
        /// Method to search the nearest vector objects.
        /// </summary>
        /// <param name="vector">Input vector</param>
        /// <param name="resultcount">Number of results expected</param>
        /// <returns>List of matched objects</returns>
        IEnumerable<IMatchedObject> Search(double[] vector, int resultcount);

        /// <summary>
        /// Method to search objects similar to the input text object
        /// </summary>
        /// <param name="textObject">Input text object</param>
        /// <param name="resultcount">Number of results expected</param>
        /// <returns>List of matched objects</returns>
        IEnumerable<IMatchedObject> Search(ITextObject textObject, int resultcount);

        /// <summary>
        /// Required length of the vector for the store
        /// </summary>
        int VectorLength { get; }

        /// <summary>
        /// Saves the vector database to specificed file path.
        /// </summary>
        /// <param name="filepath">Vector DB file path</param>
        void Save(string filepath);
    }

    /// <summary>
    /// Represents a tool that can be executed with right parameters passed through the context.
    /// </summary>
    public interface IFunctionTool
    {
        /// <summary>
        /// Gets Name of the tool
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets Description of the tool including parameters needed to execute.
        /// </summary>
        FunctionDescriptor Descriptor { get; }

        /// <summary>
        /// Executes this tool with the given execution context asynchronously
        /// </summary>
        /// <param name="context">Execution context that holds parameter values required to execute this tool.</param>
        /// <returns>Outcome of the execution, for a complex object it will return a json string.</returns>
        Task<string> ExecuteAsync(ExecutionContext context);
    }

    /// <summary>
    /// Represents a function toolset that provides multiple functions
    /// </summary>
    public interface IFunctionToolSet : IEnumerable<IFunctionTool>
    {
        /// <summary>
        /// Returns a list of functions supported by the tool
        /// </summary>
        /// <returns>A list of function descriptors</returns>
        IEnumerable<FunctionDescriptor> GetFunctions();

        /// <summary>
        /// Executes a given function with its arguments
        /// </summary>
        /// <param name="functionName">Function name to execute</param>
        /// <param name="context">Execution context object containing all relevant arguments for execution.</param>
        /// <returns>Output as string, this can be a JSON string for complex object.</returns>
        Task<string> ExecuteAsync(string functionName, ExecutionContext context);

        /// <summary>
        /// Searches a function with the given name in this toolset and returns the 
        /// corresponding tool.
        /// </summary>
        /// <param name="functionName">Name of the function to wrap as tool.</param>
        /// <returns>IFunctionTool corresponding to the given name.</returns>
        IFunctionTool GetTool(string functionName);

        /// <summary>
        /// Gets the tool with specific name.
        /// </summary>
        /// <param name="name">Name of the tool</param>
        /// <returns>IFunctionTool if exists in the collection, else throws KeyNotFoundException.</returns>
        IFunctionTool this[string name] { get; }
    }

    /// <summary>
    /// Represents a conversation chain.
    /// </summary>
    public interface IConversation
    {
        /// <summary>
        /// ID of the conversation, passed during creation of this object.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Append message to the conversation.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="role">Role of the messanger</param>
        void AppendMessage(string message, Role role);

        /// <summary>
        /// Append message to the conversation.
        /// </summary>
        /// <param name="message">Message text</param>
        void AppendMessage(ChatMessage message);

        /// <summary>
        /// Adds context to the conversation by providing external data source.
        /// </summary>
        /// <param name="context">List of Text Objects as external data source</param>
        void AddContext(IEnumerable<ITextObject> context);

        /// <summary>
        /// Adds context to the conversation by providing external data source.
        /// </summary>
        /// <param name="contextStore">Vector store as context</param>
        void AddContext(IVectorStore contextStore);

        /// <summary>
        /// Adds a toolset to the conversation, so that it can execute appropriate
        /// function from the toolset to generate response.
        /// </summary>
        /// <param name="toolSet">An IFunctionToolSet object.</param>
        void AddToolSet(IFunctionToolSet toolSet);

        /// <summary>
        /// Gets the response from AI on the current conversation asynchronously.
        /// </summary>
        /// <param name="temperature">A value between 0 to 1, that controls randomness of the response. 
        /// Higher temperature will lead to more randomness. Lower temperature will be more deterministic.</param>
        /// <returns>A ChatMessage or FunctionCallMessage object.</returns>
        Task<ChatMessage> GetResponseAsync(double temperature);
    }

    /// <summary>
    /// Represents available transformers
    /// </summary>
    public enum TransformerType
    {
        /// <summary>
        /// Uses text embedding using OpenAI API. The API key of OpenAI must be set as 'OPENAI_API_KEY'
        /// Environment variable.
        /// </summary>
        OpenAIEmbedding
    }

    /// <summary>
    /// Generative AI Service interface.
    /// </summary>
    public interface IGenerativeAIService
    {
        /// <summary>
        /// Creates an instance of a OpenAI Language Model
        /// </summary>
        /// <param name="model">OpenAI model name</param>
        /// <param name="apikey">API Key, if passed empty string then it will try to get the
        /// the key using environment variable OPENAI_API_KEY.</param>
        /// <returns></returns>
        ILanguageModel CreateOpenAIModel(string model, string apikey);

        /// <summary>
        /// Creates a new Vector store with a given transformer. A transformer transforms a text object
        /// to vector.
        /// </summary>
        /// <param name="transformer">Type of transformer. If transformer is null, it will create use default
        /// transformer using Bag of words.</param>
        /// <returns>Returns vector store</returns>
        IVectorStore CreateVectorStore(IVectorTransformer transformer);

        /// <summary>
        /// Instantiates Vector Store by deserializing a given vdb file. 
        /// </summary>
        /// <param name="vdbfile">A file with vdb extension for vector store.</param>
        /// <returns>IVectorStore object for a given vdb file</returns>
        IVectorStore DeserializeVectorStore(string vdbfile);

        /// <summary>
        /// Creates a vector transformer of a given type.
        /// </summary>
        /// <param name="type">Type of transformer such as BagOfWords</param>
        /// <returns></returns>
        IVectorTransformer CreateVectorTransformer(TransformerType type);

        /// <summary>
        /// Other services or modules can implement a specific type of transformer and register
        /// its cunstructor method with this service.
        /// </summary>
        /// <param name="type">Transformer type</param>
        /// <param name="constructor">A constructor to return specific transformer type</param>
        void RegisterTransformerConstructor(TransformerType type, Func<IVectorTransformer> constructor);

        /// <summary>
        /// Creates a conversation Object with a given context Id.
        /// </summary>
        /// <param name="contextId">Unique Id to track the conversation context.</param>
        /// <param name="languageModel">Languagel Model to be used for conversation</param>
        /// <returns>New Conversation Object</returns>
        IConversation CreateConversation(string contextId, ILanguageModel languageModel);

        /// <summary>
        /// Retrives existing conversation of specific context.
        /// </summary>
        /// <param name="contextId">Conversation context id</param>
        /// <returns>Existing Conversation Object or null</returns>
        IConversation GetConversation(string contextId);
    }
}
