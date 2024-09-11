using ProtoBuf;
using System.Collections.Generic;
using VRage.Game;

namespace NexusSyncMod.Respawn
{
    [ProtoContract]
    public class ClientGridBuilder
    {
        [ProtoMember(1, IsRequired = true)]
        public List<MyObjectBuilder_CubeGrid> Grids = new List<MyObjectBuilder_CubeGrid>();


        public ClientGridBuilder() { }
    }
}
