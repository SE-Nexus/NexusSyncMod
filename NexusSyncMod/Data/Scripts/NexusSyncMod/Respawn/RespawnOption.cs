using ProtoBuf;

namespace NexusSyncMod.Respawn
{
    [ProtoContract]
    public class RespawnOption
    {
        [ProtoMember(1)]
        public long RespawnGridID;

        [ProtoMember(2)]
        public long RespawnBlockID;


        public RespawnOption() { }
    }
}
