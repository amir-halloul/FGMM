using System.Linq;
using System.Globalization;
using FGMM.SDK.Server.RPC;
using CitizenFX.Core;

namespace FGMM.Server.RPC
{
    public class Client : IClient
    {
        public int Handle { get; }

        public string Name { get; }

        public string License { get; }

        public long? SteamId { get; }

        public string EndPoint { get; }

        public int Ping { get; }

        public Client(int handle)
        {
            this.Handle = handle;

            var player = new PlayerList()[this.Handle];

            this.Name = player.Name;
            this.License = player.Identifiers["license"];
            this.SteamId = player.Identifiers.Contains("steam") ? long.Parse(player.Identifiers["steam"], NumberStyles.HexNumber) : default(long?);
            this.EndPoint = player.EndPoint;
            this.Ping = player.Ping;
        }

        public IRpcTrigger Event(string @event) => RpcManager.Event(@event);
    }
}
