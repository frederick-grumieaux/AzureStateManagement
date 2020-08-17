using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

namespace Asm.Implementation.Cosmos.Containers
{
    /// <summary>
    /// Container used for basic stuff like virtual collections, and parameter storage.
    /// </summary>
    public interface IPrimaryContainer
    {
        /// <summary>
        /// Returns the container to read/write stuff to.
        /// </summary>
        Task<Container> Container();
    }

    public class PrimaryContainer : IPrimaryContainer
    {
        protected readonly Connector Connector;

        public PrimaryContainer(Connector connector)
        {
            Connector = connector;
        }

        public async Task<Container> Container()
        {
            const string COL_NAME = "asm.primary";
            var db = await Connector.Database();
            var response = await db.CreateContainerIfNotExistsAsync(new ContainerProperties(COL_NAME, "/pk")
            {
                DefaultTimeToLive = -1,
                PartitionKeyPath = "/pk",
                PartitionKeyDefinitionVersion = PartitionKeyDefinitionVersion.V2,
                ConflictResolutionPolicy = new ConflictResolutionPolicy
                {
                    Mode = ConflictResolutionMode.Custom,
                    ResolutionProcedure = $"dbs/{db.Id}/colls/{COL_NAME}/sprocs/conflictResolutionMode"
                },
                IndexingPolicy = new IndexingPolicy
                {
                    Automatic = true,
                    IndexingMode = IndexingMode.Consistent
                },
            });

            return response.Container;
        }
    }
}
