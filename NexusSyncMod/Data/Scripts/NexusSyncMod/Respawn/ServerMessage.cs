using ProtoBuf;
using System.Collections.Generic;

namespace NexusSyncMod.Respawn
{
    [ProtoContract]
    public class ServerMessage
    {
        [ProtoMember(1)]
        public readonly ulong PlayerSteamID;

        [ProtoMember(2)]
        public readonly string ServerIP;

        [ProtoMember(3)]
        public readonly long MessageAuthentication;

        [ProtoMember(4)]
        public bool ClearRenderedGrids = false;

        [ProtoMember(6, IsRequired = true)]
        public List<ClientGridBuilder> GridBuilders = new List<ClientGridBuilder>();

        public ServerMessage(ulong SteamID, string Server, long Authentication)
        {
            PlayerSteamID = SteamID;
            ServerIP = Server;
            MessageAuthentication = Authentication;
        }

        public ServerMessage() { }
    }
}
