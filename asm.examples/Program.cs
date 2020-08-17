using System;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var services = new ServiceCollection();
            Installers.Installer.Install(services);

            var container = services.BuildServiceProvider();
            var service = container.GetRequiredService<BasicParameterExample>();
            var task = service.RunTest();

            task.Wait();
        }
    }
}
