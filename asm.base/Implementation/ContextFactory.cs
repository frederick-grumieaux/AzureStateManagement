using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Asm.Implementation
{
    public class ContextFactory : IContextFactory
    {
        public Context CreateNew()
        {
            return new BaseContext();
        }

        public class BaseContext : Context
        {
            public BaseContext()
            {
                ActivityId = Guid.NewGuid();
                Log = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
                Data = new ConcurrentDictionary<string, object>();
            }
        }
    }
}
