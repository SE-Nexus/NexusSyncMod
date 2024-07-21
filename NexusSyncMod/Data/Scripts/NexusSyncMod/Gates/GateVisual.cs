using ProtoBuf;
using VRageMath;

namespace NexusSyncMod.Gates
{
    [ProtoContract]
    public class GateVisual
    {
        [ProtoMember(10)]
        public Vector3D Center;

        [ProtoMember(20)]
        public Vector3D Direction;

        [ProtoMember(40)]
        public float Size = 800;

        [ProtoMember(50)]
        public string ParticleEffect;

        public GateVisual(Vector3D Center, Vector3D Direction, string ParticleEffect, float Size = 800)
        {
            this.Center = Center;
            this.Direction = Direction;
            this.Size = Size;
            this.ParticleEffect = ParticleEffect;
        }

        public GateVisual() { }

    }
}
