using ProtoBuf;
using System.ComponentModel;
using VRageMath;

namespace NexusAPI.ConfigAPI
{
    [ProtoContract]
    public class SectorData
    {
        [ProtoMember(1), DefaultValue("NewSector")]
        public string SectorName { get; set; } = "NewSector";

        [ProtoMember(2), DefaultValue("NewSectorDescription")]
        public string SectorDescription { get; set; } = "NewSectorDescription";

        [ProtoMember(3)]
        public byte OnServerID { get; set; }

        [ProtoMember(4), DefaultValue(SectorShapeEnum.Sphere)]
        public SectorShapeEnum SectorShape { get; set; } = SectorShapeEnum.Sphere;


        [ProtoMember(5)]
        public double X { get; set; }

        [ProtoMember(6)]
        public double Y { get; set; }

        [ProtoMember(7)]
        public double Z { get; set; }


        [ProtoMember(8)]
        public double DX { get; set; }

        [ProtoMember(9)]
        public double DY { get; set; }

        [ProtoMember(10)]
        public double DZ { get; set; }



        [ProtoMember(11)]
        public float RadiusKM { get; set; }

        [ProtoMember(12)]
        public float RingRadiusKM { get; set; }

        [ProtoMember(13)]
        public ushort SectorID { get; set; }

        [ProtoMember(14)]
        public string SectorBoundaryScript { get; set; }

        [ProtoMember(16)]
        public bool HiddenSector { get; set; }

        [ProtoMember(17), DefaultValue(true)]
        public bool EnableSectorInfoProvider { get; set; } = true;

        [ProtoMember(18)]
        public SectorBorderTexture BorderTexture { get; set; }

        [ProtoMember(19, IsRequired = true)]
        public Color BorderColor { get; set; } = new Color(255, 255, 255, 255);

        public SectorData()
        {

        }
    }
}
