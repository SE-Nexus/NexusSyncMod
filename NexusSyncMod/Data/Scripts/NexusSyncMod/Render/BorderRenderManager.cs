using Draygo.API;
using NexusSyncMod.Render.Shapes;
using Sandbox.ModAPI;
using System.Collections.Generic;
using VRage.ModAPI;
using VRageMath;

namespace NexusSyncMod.Render
{
    internal class BorderRenderManager
    {
        private const string MaterialPrefix = "NexusBorder";
        private const double MaxRaycastDistance = 10000;

        private readonly List<BorderShape> borders = new List<BorderShape>();

        private HudAPIv2 hudApi;
        private RenderStatusHud statusHud;

        public bool UpdateCameraPosition { get; set; } = true;

        public void Add(BoundingSphereD sphere, Color color, BorderTexture texture, string name)
        {
            borders.Add(new SphereBorder(sphere, MaterialPrefix + texture, color, name));
        }

        public void Add(BoundingBoxD box, Color color, BorderTexture texture, string name)
        {
            borders.Add(new BoxBorder(box, MaterialPrefix + texture, color, name));
        }

        public void Add(Vector3D center, Vector3D up, double majorRadius, double minorRadius, Color color, BorderTexture texture, string name)
        {
            borders.Add(new TorusBorder(center, up, majorRadius, minorRadius, MaterialPrefix + texture, color, name));
        }

        public void Init()
        {
            hudApi = new HudAPIv2(OnHudReady);
        }

        public void Unload()
        {
            if (hudApi != null)
                hudApi.Unload();
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

    }
}
