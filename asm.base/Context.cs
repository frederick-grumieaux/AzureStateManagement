using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Asm
{
    public class Context
    {
        public virtual ILogger Log { get; protected set; }
        public virtual Guid ActivityId { get; protected set; } = Guid.NewGuid();
        protected virtual ConcurrentDictionary<string, object> Data { get; set; } 
    }

    public interface IContextFactory
    {
        Context CreateNew();
    }
}
