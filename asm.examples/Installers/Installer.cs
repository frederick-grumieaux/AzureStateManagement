using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Examples.Installers
{
    public static class Installer
    {
        public static void Install(IServiceCollection services)
        {
            //Install libraries
            Asm.Installers.LibraryInstaller.Install(services);

            //Override config...
            services.AddSingleton<Asm.MicroService>(_ => new Asm.MicroService { Name = "ExampleService" });
            services.AddSingleton<Asm.Cosmos.CosmosDbConnectionConfig>(_ => new Cosmos.CosmosDbConnectionConfig
            {
                //This example account will be removed after 30 days, and is completly free.
                Key = "j4Js8SM9KahK4KpJHeVNJcd4CbiTFPKikndHUQacqH0VDzE4AqMIykp1e8VkTUVYFStW4PPVrnU36sxIidliWA==",
                Endpoint = "AccountEndpoint=https://6d990146-0ee0-4-231-b9ee.documents.azure.com:443/;AccountKey=j4Js8SM9KahK4KpJHeVNJcd4CbiTFPKikndHUQacqH0VDzE4AqMIykp1e8VkTUVYFStW4PPVrnU36sxIidliWA==;"
            });

            //install current assembly
            services.AddTransient<BasicParameterExample>();
        }
    }
}
