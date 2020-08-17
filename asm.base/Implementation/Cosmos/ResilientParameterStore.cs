using Asm.Cosmos;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Asm.Implementation.Cosmos.Containers;
using Microsoft.Azure.Cosmos;

namespace Asm.Implementation.Cosmos
{
    public class ResilientParameterStore : Asm.Cosmos.IResilientParameterStore
    {
        protected static string ToFullID(string key) => $"p.{key}";

        protected readonly IPrimaryContainer PrimaryContainer;

        public ResilientParameterStore(
            IPrimaryContainer container
            )
        {
            PrimaryContainer = container;
        }


        protected void ValidateDocument(Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (string.IsNullOrWhiteSpace(document.ID)) throw new ArgumentException("The id cannot be null or empty");
            if (string.IsNullOrWhiteSpace(document.PartitionKey)) throw new ArgumentException($"The partitionkey cannot be empty");
        }

        public async Task<ParameterDocument<T>> Create<T>(string key, T data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var container = await PrimaryContainer.Container();

            var id = ToFullID(key);
            var document = new ParameterDocument<T>
            {
                Data = data,
                ID = id,
                PartitionKey = id
            };

            var result = await container.CreateItemAsync(
                document,
                new PartitionKey(document.ID),
                new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = true,
                    ConsistencyLevel = ConsistencyLevel.Session
                });

            return result.Resource;
        }

        public async Task Delete<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var container = await PrimaryContainer.Container();

            var fullKey = ToFullID(key);

            await container.DeleteItemAsync<ParameterDocument<T>>(
                fullKey,
                new PartitionKey(fullKey),
                new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = true,
                    ConsistencyLevel = Microsoft.Azure.Cosmos.ConsistencyLevel.Session
                })
                .ConfigureAwait(false);
        }

        public async Task Delete<T>(ParameterDocument<T> document)
        {
            ValidateDocument(document);

            var container = await PrimaryContainer.Container();
            await container.DeleteItemAsync<ParameterDocument<T>>(
                document.ID,
                new PartitionKey(document.PartitionKey),
                new ItemRequestOptions
                {
                    //EnableContentResponseOnWrite = true,
                    ConsistencyLevel = Microsoft.Azure.Cosmos.ConsistencyLevel.Session,
                    IfMatchEtag = document.ETag
                })
                .ConfigureAwait(false);
        }

        public async Task<ParameterDocument<T>> Get<T>(string key)
        {
            var container = await PrimaryContainer.Container();

            var query = new QueryDefinition(@"SELECT * FROM collection c WHERE c.id = @id")
                .WithParameter("@id", ToFullID(key));

            using var iterator = container.GetItemQueryIterator<ParameterDocument<T>>(query,
                null,
                new QueryRequestOptions {
                    PartitionKey = new PartitionKey(ToFullID(key))
                });
            while (iterator.HasMoreResults)
            {
                var records = await iterator.ReadNextAsync();
                return records.Resource.FirstOrDefault();
            }

            return null;
        }

        public async Task<ParameterDocument<T>> Update<T>(ParameterDocument<T> document)
        {
            ValidateDocument(document);

            var container = await PrimaryContainer.Container();

            var result = await container.UpsertItemAsync(document,
                new PartitionKey(document.PartitionKey),
                new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = true,
                    IfMatchEtag = document.ETag
                });

            return result.Resource;
        }
    }
}
