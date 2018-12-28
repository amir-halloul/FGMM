using System;
using System.Threading.Tasks;
using FGMM.SDK.Client.Events;
using FGMM.SDK.Client.RPC;
using FGMM.SDK.Core.Diagnostics;
using CitizenFX.Core;

namespace FGMM.SDK.Client.Services
{
    public class Service
    {
        protected ILogger Logger { get; set; }
        protected IEventManager Events { get; set; }
        protected IRpcHandler Rpc { get; set; }

        public Service(ILogger logger, IEventManager events, IRpcHandler rpc)
        {
            Logger = logger;
            Events = events;
            Rpc = rpc;
            Started();
        }

        public virtual Task Started() => Task.FromResult(0);

        protected async Task Delay(int ms)
        {
            await BaseScript.Delay(ms);
        }

        protected async Task Delay(TimeSpan delay)
        {
            await BaseScript.Delay((int)delay.TotalMilliseconds);
        }
    }
}
