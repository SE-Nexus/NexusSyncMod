using System;
using VRageMath;

namespace NexusSyncMod.Render.Shapes
{
    internal class SphereBorder : BorderShape
    {
        private readonly BoundingSphereD sphere;
        private readonly int numTextures;

        private readonly double maxDistance2;
        private readonly double minDistance2;

        public SphereBorder(BoundingSphereD sphere, string texture, Color color, string name) : base(texture, color, name)
        {
            this.sphere = sphere;

            maxDistance2 = sphere.Radius + TransitionStart;
            maxDistance2 *= maxDistance2;
            minDistance2 = Math.Max(sphere.Radius - TransitionStart, 0);
            minDistance2 *= minDistance2;

            numTextures = (int)Math.Max(TransitionStart / BorderTextureSize, 1);
        }

        public override bool Draw(Vector3D camera)
        {
            Vector3D toSurface;
            if (!IsInsideRenderDistance(camera, out toSurface))
                return false;

            double azimuth;
            double elevation;
            Vector3D.GetAzimuthAndElevation(toSurface, out azimuth, out elevation);


            // radians = arc length / radius
            double elevationStep = (BorderTextureSize / sphere.Radius);

            int renderCount = 0;

            for (int e = -numTextures; e <= numTextures; e++)
            {
                double eComp =  (elevationStep * e);
                eComp += elevation;

                ClampToGrid(ref eComp, elevationStep);

                if (Math.Abs(eComp) >= Pi / 2)
                    continue;

                double azimuthStep = GetStepSizeForElevation(eComp);
                if (azimuthStep >= Pi)
                    continue;

                for (int a = -numTextures; a <= numTextures; a++)
                {
                    double aComp = azimuthStep * a;
                    if (Math.Abs(aComp) >= Pi)
                        continue;

                    aComp += azimuth;


                    ClampToGrid(ref aComp, azimuthStep);

                    Vector3D.CreateFromAzimuthAndElevation(aComp, eComp, out toSurface);
                    Vector3D pos = (toSurface * sphere.Radius) + sphere.Center;
                    

                    DrawTexture(pos, toSurface);
                    renderCount++;

                }

            }

            return true;
        }

        private double ClampTwoPi(double x)
        {
            if (x < 0)
            {
                int wholeEntries = (int)(-x / Pi2);
                x += (wholeEntries + 1) * Pi2;
            }
            return x % Pi2;
        }

        // Source: Vector3D.CreateFromAzimuthAndElevation
        private void CreateFromElevation(double elevation, out Vector3D direction)
        {
            MatrixD matrix = MatrixD.CreateRotationX(elevation);
            direction = Vector3D.Forward;
            Vector3D.TransformNormal(ref direction, ref matrix, out direction);
        }

        private double GetStepSizeForElevation(double elevation)
        {
            Vector3D direction;
            CreateFromElevation(elevation, out direction);
            direction *= sphere.Radius;

            // Source: Vector3D.Distance
            // Pretend Y is zero to get the radius at this elevation
            double radius2 = direction.X * direction.X + direction.Z * direction.Z;

            // radians = arc length / radius
            return VectorTools.MakeAngleLoopable(BorderTextureSize / Math.Sqrt(radius2));
        }

        private void ClampToGrid(ref double angle, double grid)
        {
            angle = Math.Round(angle / grid) * grid;
        }


        protected void ClampToSurface(ref Vector3D position, out Vector3D normal)
        {
            Vector3D local = position - sphere.Center;
            normal = local.Normalized();
            position = normal * sphere.Radius;
        }

        protected bool IsInsideRenderDistance(Vector3D position, out Vector3D toSurface)
        {
            Vector3D local = position - sphere.Center;
            double dist2 = local.LengthSquared();
            if (!IsInsideRenderDistance(dist2))
            {
                toSurface = Vector3D.Zero;
                return false;
            }

            toSurface = local / Math.Sqrt(dist2);
            return true;
        }

        private bool IsInsideRenderDistance(double distance2)
        {
            return distance2 < maxDistance2 && distance2 > minDistance2;
        }

        public override bool IsInside(Vector3D position)
        {
            return sphere.Contains(position) == ContainmentType.Contains;
        }

        public override bool Raycast(RayD ray, out double distance)
        {
            distance = double.PositiveInfinity;

            // https://en.wikipedia.org/wiki/Line%E2%80%93sphere_intersection
            // u = ray.Direction
            // o = ray.Position
            // c = sphere.Center
            // r = sphere.Radius

            Vector3D sphereToRay = ray.Position - sphere.Center;
            double sphereDotRay = ray.Direction.Dot(sphereToRay);

            double delta = (sphereDotRay * sphereDotRay) - (sphereToRay.LengthSquared() - (sphere.Radius * sphere.Radius));
            if (delta < 0)
                return false; // No intersection

            double sqrtDelta = Math.Sqrt(delta);

            sphereDotRay *= -1;
            
            double a = sphereDotRay + sqrtDelta;
            double b = sphereDotRay - sqrtDelta;
            if (a < 0)
            {
                if (b < 0)
                    return false;
                distance = b;
                return true;
            }

            if (b < 0)
            {
                distance = a;
                return true;
            }

            distance = Math.Min(a, b);
            return true;
        }
    }
}
