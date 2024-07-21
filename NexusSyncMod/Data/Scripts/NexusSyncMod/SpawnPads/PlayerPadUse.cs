using ProtoBuf;
using System;

namespace NexusSyncMod.SpawnPads
{
    [ProtoContract]
    public class PlayerPadUse
    {
        [ProtoMember(1)]
        public DateTime LastUse;

        [ProtoMember(2)]
        public int Count;

        public PlayerPadUse()
        {
            LastUse = DateTime.Now;
            Count = 1;
        }

    }
}
