using VRageMath;

namespace NexusSyncMod.Nexus
{
    public class Sector
    {
        public readonly string Name;

        public readonly string IPAddress;

        public readonly int Port;

        public readonly bool IsGeneralSpace;

        public readonly Vector3D Center;

        public readonly double Radius;

        public readonly int ServerID;

        public Sector(string Name, string IPAddress, int Port, bool IsGeneralSpace, Vector3D Center, double Radius, int ServerID)
        {
            this.Name = Name;
            this.IPAddress = IPAddress;
            this.Port = Port;
            this.IsGeneralSpace = IsGeneralSpace;
            this.Center = Center;
            this.Radius = Radius;
            this.ServerID = ServerID;
        }

    }
}
