using ProtoBuf;
using System.Collections.Generic;

namespace NexusSyncMod.Players
{
    [ProtoContract]
    public class OnlinePlayerData
    {
        [ProtoMember(10, IsRequired = true)] public List<OnlinePlayer> OnlinePlayers = new List<OnlinePlayer>();

        [ProtoMember(12)] public int currentServerID;
    }
}
