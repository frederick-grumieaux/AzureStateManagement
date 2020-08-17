using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Asm.Cosmos
{
    public interface ICollectionDefinition
    {

    }
    /// <summary>
    /// Contains the definition of a virtual collection.
    /// </summary>
    public interface IVirtualCollectionDefintion
    {
        /// <summary>
        /// The name of the collection.
        /// The "actual" collection in cosmos-db can contain multiple virtual collections.
        /// A virtual collection consists of a namespace and name separated with a dot.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The namespace of the collection.
        /// </summary>
        string NameSpace { get; }
        /// <summary>
        /// Information about the actuall collection housing the virtual collection.
        /// </summary>
        ICollectionDefinition ActualCollection { get; }
    }
    /// <summary>
    /// A virtual change feed allows to wait (without blocking) for the next change in a virtual collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IVirtualChangeFeed<T> : IAsyncEnumerable<T>
    {
        /// <summary>
        /// The name of the change feed (or null for annonymous change feeds)
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The filter limiting the objects returned by the change feed.
        /// </summary>
        Expression<T> Filter { get; }
        /// <summary>
        /// Reference to the virtual collection which is the source of this change feed.
        /// </summary>
        IVirtualCollection<T> Collection { get; }
    }

    /// <summary>
    /// Contains a set of objects which can be queried performantly.
    /// If possible, all objects will reside in the same partition, however 
    /// should the size of the partition grow it will automatically scale to 
    /// multiple partitions.
    /// </summary>
    public interface IVirtualCollection<T> : IAsyncEnumerable<T>
    {
        IVirtualChangeFeed<T> CreateChangeFeed(Expression<T> filter);
        IVirtualChangeFeed<T> GetOrCreateFeed(Expression<T> filter);

        IVirtualCollectionDefintion Definition { get; }
    }

    /// <summary>
    /// Creates change feeds given a virtual collection.
    /// </summary>
    public interface IChangeFeedFactory
    {
        Task<IVirtualChangeFeed<T>> CreateAnnonymousFeed<T>(IVirtualCollection<T> collection, Expression<Func<T, bool>> filter);
        Task<IVirtualChangeFeed<T>> GetOrCreateFeed<T>(string name, IVirtualChangeFeed<T> collection, Expression<Func<T, bool>> filter);
    }

    /// <summary>
    /// Allows to create virtual collections.
    /// </summary>
    public interface IVirtualCollectionFactory
    {
        /// <summary>
        /// Gets or creates the virtual collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="definition"></param>
        /// <returns></returns>
        Task<IVirtualCollection<T>> GetOrCreateCollection<T>(IVirtualCollectionDefintion definition);
    }

    public interface IFallBackLogger : Microsoft.Extensions.Logging.ILogger
    { }

    public interface IResilientParameterStore
    {
        /// <summary>
        /// Retrieves a parameter stored in the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<ParameterDocument<T>> Get<T>(string key);
        /// <summary>
        /// Creates a new parameter in the database with a given key.
        /// </summary>
        /// <remarks>Throws an exception if the parameter is not found.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key of the parmaeter.</param>
        /// <param name="data"></param>
        /// <returns>The stored data.</returns>
        Task<ParameterDocument<T>> Create<T>(string key, T data);
        /// <summary>
        /// Updates a parameter in the database with the new value.
        /// </summary>
        /// <remarks>returns the updated value</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key of the parameter to store.</param>
        /// <param name="data">The new data of the parameter.</param>
        /// <returns>The updated object.</returns>
        Task<ParameterDocument<T>> Update<T>(ParameterDocument<T> data);
        /// <summary>
        /// Deletes the parameter from the database.
        /// </summary>
        /// <remarks>Throws an exception if the record does not exist.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key of the parameter</param>
        /// <returns>The data that was deleted</returns>
        Task<ParameterDocument<T>> Delete<T>(ParameterDocument<T> document);
        /// <summary>
        /// Deletes the parameter, if and only if the parameter has not changed in the mean time.
        /// </summary>
        /// <remarks>Throws an exception if the document does not exist, or if it has changed in the mean time.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        /// <returns>The removed parameter.</returns>
        Task<ParameterDocument<T>> Delete<T>(string key);
    }

    /// <summary>
    /// This class needs to be provided by the assembly using the database.
    /// </summary>
    public class CosmosDbConnectionConfig
    {
        /// <summary>
        /// The URI to the cosmos-db
        /// </summary>
        public Uri Uri { get; set; }
        /// <summary>
        /// The key to connect to the cosmos db
        /// </summary>
        public string Key { get; set; }
    }

    public class IMicroService
    {
        public string Name { get; set; }
    }
}
