using Draygo.API;
using NexusAPI.ConfigAPI;
using NexusSyncMod.Gates;
using NexusSyncMod.Render.Shapes;
using ProtoBuf;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using VRage.ModAPI;
using VRageMath;

namespace NexusSyncMod.Render
{
    internal class BorderRenderManager
    {
        private const string MaterialPrefix = "NexusBorder";
        private const ushort BorderNetId = 2938;
        private const double MaxRaycastDistance = 10000;

        private readonly List<BorderShape> borders = new List<BorderShape>();

        private HudAPIv2 hudApi;
        private RenderStatusHud statusHud;


        internal void InitNetwork()
        {
            if (!MyAPIGateway.Session.IsServer)
                MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(BorderNetId, MessageHandler);
        }

        private void MessageHandler(ushort id, byte[] data, ulong sender, bool fromServer)
        {
            if (!fromServer)
                return;

            ClientSectorMessage receivedMessage = MyAPIGateway.Utilities.SerializeFromBinary<ClientSectorMessage>(data);

            borders.Clear();

            foreach(SectorData sector in receivedMessage.Sectors)
            {
                Vector3D position = new Vector3D(sector.X, sector.Y, sector.Z);
                double size = sector.RadiusKM * 1000;
                string texture = MaterialPrefix + sector.BorderTexture;
                Color color = sector.BorderColor;
                string name = sector.SectorName;

                BorderShape border;
                switch (sector.SectorShape)
                {
                    case SectorShapeEnum.Sphere:
                        border = new SphereBorder(new BoundingSphereD(position, size), texture, color, name);
                        break;
                    case SectorShapeEnum.Cuboid:
                        Vector3D max = new Vector3D(sector.DX, sector.DY, sector.DZ);
                        Vector3D min = Vector3D.Min(position, max);
                        max = Vector3D.Max(position, max);
                        border = new BoxBorder(new BoundingBoxD(min, max), texture, color, name);
                        break;
                    case SectorShapeEnum.Torus:
                        Vector3D up = new Vector3D(sector.DX, sector.DY, sector.DZ);
                        double minorRadius = sector.RingRadiusKM * 1000;
                        border = new TorusBorder(position, up, size, minorRadius, texture, color, name);
                        break;
                    default:
                        continue;
                }

                borders.Add(border);
            }
        }

        public void InitHud()
        {
            hudApi = new HudAPIv2(OnHudReady);
        }

        public void Unload()
        {
            if (hudApi != null)
                hudApi.Unload();

            borders.Clear();
            MyAPIGateway.Multiplayer.UnregisterSecureMessageHandler(BorderNetId, MessageHandler);
        }

        private void OnHudReady()
        {
            statusHud = new RenderStatusHud(Color.Red);
        }

        public void Draw()
        {
            IMyCamera camera = MyAPIGateway.Session?.Camera;
            if (camera == null)
                return;

            Vector3D cameraPosition = camera.Position;

            RayD ray;
            double speed;
            bool hasMovement = TryGetMovement(out ray, out speed);

            BorderShape minBorder = null;
            double minDistance = double.PositiveInfinity;

            foreach (BorderShape border in borders)
            {
                double distance;
                if(border.Draw(cameraPosition) && hasMovement && statusHud != null && border.Raycast(ray, out distance) && distance < minDistance && distance < MaxRaycastDistance)
                {
                    minBorder = border;
                    minDistance = distance;
                }
            }

            if (minBorder != null)
            {
                double seconds = minDistance / speed;
                statusHud.Update(minDistance, seconds, minBorder.Name, minBorder.IsInside(ray.Position));
            }
            else if(statusHud != null)
            {
                statusHud.Visible = false;
            }
        }

        private bool TryGetMovement(out RayD ray, out double speed)
        {
            ray = new RayD();
            speed = 0;

            IMyEntity entity = GetControlledEntity();
            if (entity == null)
                return false;
            entity = entity.GetTopMostParent();
            if (entity.Physics == null)
                return false;
            Vector3D velocity = entity.Physics.LinearVelocity;
            if (Vector3D.IsZero(velocity, 1))
                return false;

            ray.Position = entity.WorldMatrix.Translation;
            speed = velocity.Length();
            ray.Direction = velocity / speed;
            return true;

        }

        private IMyEntity GetControlledEntity()
        {
            return MyAPIGateway.Session?.Player?.Controller?.ControlledEntity?.Entity;
        }


        [ProtoContract]
        private class ClientSectorMessage
        {
            [ProtoMember(1)]
            public SectorData[] Sectors { get; set; }

            public ClientSectorMessage() { }

            public ClientSectorMessage(SectorData[] sectors)
            {
                Sectors = sectors;
            }
        }
    }
}
