using ProtoBuf;

namespace NexusAPI.ConfigAPI
{
    [ProtoContract]
    public enum SectorShapeEnum
    {
        Sphere, //Center and radius
        Cuboid, //Two points opposite corners define space
        Torus //Define Center, Radius, Ring Radius, Direction Vector (Perpendicular to Radius)
    }
}
