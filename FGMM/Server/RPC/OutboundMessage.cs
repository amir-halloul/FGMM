using System;
using System.Collections.Generic;

namespace FGMM.Server.RPC
{
    public class OutboundMessage
    {
        public Client Target { get; set; }

        public string Event { get; set; }

        public List<string> Payloads { get; set; } = new List<string>();

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
