using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Asm.Cosmos;

namespace Asm.Examples
{
    public class BasicParameterExample
    {
        protected readonly IResilientParameterStore Parameters;

        public BasicParameterExample(IResilientParameterStore parameterStore)
        {
            Parameters = parameterStore;
        }

        public async Task RunTest()
        {
            const string PARAM_KEY = "BasicParameterExample.TextParam";

            //Get the parameter (and since we have not yet stored it, expect the parameter to be null)
            try
            {
                Console.WriteLine($"Attempting to retrieve parameter: {PARAM_KEY}");
                var param = await Parameters.Get<string>(PARAM_KEY);
                if (param == null)
                    Console.WriteLine($"parameter {PARAM_KEY} does not exist");
                else
                    Console.WriteLine($"parameter {PARAM_KEY} has value: {param?.Data}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"exception: {ex}");
            }

            try
            {
                Console.WriteLine("Store value 'abc'");
                var param = await Parameters.Create<string>(PARAM_KEY, "abc");
                Console.WriteLine($"store succeeded: new value = {param?.Data}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"exception: {ex}");
            }

            try
            {
                Console.WriteLine("Update value...");
                var param = await Parameters.Get<string>(PARAM_KEY);
                Console.Write($"From old value: {param?.Data} -> to ");
                param.Data = DateTime.UtcNow.ToString("u");
                var updatedParam = await Parameters.Update<string>(param);
                Console.WriteLine($"new value = {param?.Data}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"exception: {ex}");
            }

            try
            {
                Console.WriteLine("delete parameter...");
                await Parameters.Delete<string>(PARAM_KEY);
                Console.WriteLine($"removed parameter");
                var createdParam = await Parameters.Create<int>(PARAM_KEY, 41);
                await Parameters.Delete(createdParam);
                Console.WriteLine($"removed parameter {createdParam?.ID}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"exception: {ex}");
            }

        }


        
    }
}
