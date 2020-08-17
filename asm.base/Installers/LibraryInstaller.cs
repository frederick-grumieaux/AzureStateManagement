using System;
using System.Collections.Generic;
using System.Text;
using Asm.Implementation.Cosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Installers
{
    public static class LibraryInstaller
    {
        public static void Install(IServiceCollection services)
        {
            services.AddSingleton<Connector>();
            services.AddSingleton<CosmosSerializer, JsonTextSerializer>();
            services.AddSingleton<Implementation.Cosmos.Containers.IPrimaryContainer, Implementation.Cosmos.Containers.PrimaryContainer>();

            services.AddTransient<IContextFactory, Implementation.ContextFactory>();
            services.AddTransient<Context>(services => services.GetService<IContextFactory>().CreateNew());
            services.AddTransient<Cosmos.IResilientParameterStore, Implementation.Cosmos.ResilientParameterStore>();
        }
    }
}
