using ProtoBuf;
using System.Collections.Generic;

namespace NexusSyncMod.Gates
{
    [ProtoContract]
    public class GateVisualData
    {


        [ProtoMember(10)]
        public List<GateVisual> AllGates = new List<GateVisual>();


        public GateVisualData(List<GateVisual> Gates)
        {
            AllGates = Gates;
        }

        public GateVisualData() { }
    }
}
