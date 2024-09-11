using ProtoBuf;
using System.Collections.Generic;

namespace NexusSyncMod.Players
{
    [ProtoContract]
    public class OnlinePlayerData
    {
        [ProtoMember(10, IsRequired = true)] public List<OnlineClientServer> OnlineServers = new List<OnlineClientServer>();

        [ProtoMember(12)] public int currentServerID;
    }
}
