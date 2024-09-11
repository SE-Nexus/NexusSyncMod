using ProtoBuf;
using System.Collections.Generic;

namespace NexusSyncMod.Players
{
    [ProtoContract]
    public class OnlineClientServer
    {
        [ProtoMember(2, IsRequired = true)] public List<OnlinePlayer> Players = new List<OnlinePlayer>();

        [ProtoMember(3)] public bool ServerRunning = false;

        [ProtoMember(10)] public int ServerID;

        [ProtoMember(11)] public string ServerName;

        public OnlineClientServer()
        {
        }
    }

}
