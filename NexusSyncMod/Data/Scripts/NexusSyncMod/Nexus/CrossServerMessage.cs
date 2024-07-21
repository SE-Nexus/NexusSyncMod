using ProtoBuf;

namespace NexusSyncMod.Nexus
{
    [ProtoContract]
    public class CrossServerMessage
    {

        [ProtoMember(1)] public readonly int ToServerID;
        [ProtoMember(2)] public readonly int FromServerID;
        [ProtoMember(3)] public readonly ushort UniqueMessageID;
        [ProtoMember(4)] public readonly byte[] Message;

        public CrossServerMessage(ushort UniqueMessageID, int ToServerID, int FromServerID, byte[] Message)
        {
            this.UniqueMessageID = UniqueMessageID;
            this.ToServerID = ToServerID;
            this.FromServerID = FromServerID;
            this.Message = Message;
        }

        public CrossServerMessage() { }
    }
}
