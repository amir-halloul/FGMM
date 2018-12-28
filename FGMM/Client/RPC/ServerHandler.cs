using System;
using CitizenFX.Core;

namespace FGMM.Client.RPC
{
    public class ServerHandler
    {
        private readonly EventHandlerDictionary events;

        public ServerHandler(EventHandlerDictionary events)
        {
            this.events = events;
        }

        public void Attach(string @event, Delegate callback)
        {
            this.events[@event] += callback;
        }

        public void Detach(string @event, Delegate callback)
        {
            this.events[@event] -= callback;
        }
    }
}
