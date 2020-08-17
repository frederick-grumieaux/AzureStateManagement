using System;
using System.Collections.Generic;
using System.Text;
using Asm.Cosmos;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Asm.Implementation.Cosmos
{
    public class Connector
    {
        protected readonly CosmosDbConnectionConfig Config;
        protected readonly IFallBackLogger Logger;
        protected readonly CosmosSerializer Serializer;
        protected readonly MicroService MicroService;

        public Connector(
            IFallBackLogger logger, 
            CosmosDbConnectionConfig config, 
            CosmosSerializer serializer,
            MicroService applicationName)
        {
            Logger = logger;
            Config = config;
            Serializer = serializer;
            MicroService = applicationName;

            client = new Lazy<Task<CosmosClient>>(CreateNewClient, System.Threading.LazyThreadSafetyMode.PublicationOnly);
            database = new Lazy<Task<Database>>(InitDatabase, System.Threading.LazyThreadSafetyMode.PublicationOnly);
        }


        protected Lazy<Task<CosmosClient>> client; 
        public virtual async ValueTask<CosmosClient> DbClient()
        {
            var task = client.Value;
            if (task.IsCompleted)
                return task.Result;
            else
                return await task;
        }
        protected virtual Task<CosmosClient> CreateNewClient()
        {
            var connector = new CosmosClient(
                Config.Endpoint,
                new CosmosClientOptions
                {
                    AllowBulkExecution = false,
                    ApplicationName = MicroService.Name,
                    //ApplicationPreferredRegions = new[] { "" },
                    //ApplicationRegion = "westheurope", //LocationNames.NorthEurope,
                    ConnectionMode = ConnectionMode.Direct,
                    ConsistencyLevel = ConsistencyLevel.Session,
                    //CustomHandlers = new RequestHandler[0],
                    EnableTcpConnectionEndpointRediscovery = true,
                    GatewayModeMaxConnectionLimit = 50,
                    HttpClientFactory = null,
                    IdleTcpConnectionTimeout = null,
                    LimitToEndpoint = false,
                    MaxRequestsPerTcpConnection = null,
                    MaxRetryAttemptsOnRateLimitedRequests = null,
                    MaxRetryWaitTimeOnRateLimitedRequests = null,
                    MaxTcpConnectionsPerEndpoint = null,
                    OpenTcpConnectionTimeout = null,
                    PortReuseMode = null,
                    //RequestTimeout
                    Serializer = Serializer,
                    //SerializerOptions = new CosmosSerializationOptions
                    //{
                    //    IgnoreNullValues = true,
                    //    Indented = false,
                    //    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    //},
                    //WebProxy = null,
                }
                );

            return Task.FromResult(connector);
        }


        protected Lazy<Task<Database>> database;
        public virtual async ValueTask<Database> Database()
        {
            var task = database.Value;
            if (task.IsCompleted)
                return task.Result;
            else
                return await task;
        }
        protected virtual async Task<Database> InitDatabase()
        {
            var dbID = MicroService.Name;

            var client = await DbClient();
            var response = await client.CreateDatabaseIfNotExistsAsync(
                dbID,
                ThroughputProperties.CreateAutoscaleThroughput(50000));

            Logger.LogInformation($"Created database {dbID} -> {response.StatusCode}");

            return response.Database;
        }
    }
}
