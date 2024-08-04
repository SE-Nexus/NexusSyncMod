using ProtoBuf;
using System.Collections.Generic;

namespace NexusSyncMod.Gates
{
    [ProtoContract]
    public class GateVisualMsg
    {


        [ProtoMember(10)]
        public List<GateVisualData> AllGates = new List<GateVisualData>();


        public GateVisualMsg(List<GateVisualData> Gates)
        {
            AllGates = Gates;
        }

        public GateVisualMsg() { }
    }
}
