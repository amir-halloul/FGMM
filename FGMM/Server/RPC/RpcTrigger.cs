using FGMM.SDK.Core.RPC;
using FGMM.SDK.Server.Diagnostics;
using CitizenFX.Core;

namespace FGMM.Server.RPC
{
    public class RpcTrigger
    {
        private readonly Logger logger;
        private readonly Serializer serializer;

        public RpcTrigger(Logger logger, Serializer serializer)
        {
            this.logger = logger;
            this.serializer = serializer;
        }

        public void Fire(OutboundMessage message)
        {
            this.logger.Debug($"Fire: \"{message.Event}\" with {message.Payloads.Count} payload(s): {string.Join(", ", message.Payloads)}");

            if (message.Target != null)
            {
                new PlayerList()[message.Target.Handle].TriggerEvent(message.Event, this.serializer.Serialize(message));
            }
            else
            {
                BaseScript.TriggerClientEvent(message.Event, this.serializer.Serialize(message));
            }
        }
    }
}
