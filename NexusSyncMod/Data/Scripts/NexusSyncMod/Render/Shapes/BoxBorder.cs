using Sandbox.ModAPI;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using VRage.Game;
using VRage.Utils;
using VRageMath;

namespace NexusSyncMod.Render.Shapes
{
    internal class BoxBorder : BorderShape
    {
        private readonly BoundingBoxD outerRenderLimit;
        private readonly BoundingBoxD innerRenderLimit;
        private readonly BoundingBoxD box;
        private readonly BoxFace[] faces = new BoxFace[6];

        public BoxBorder(BoundingBoxD box, string texture, Color color, string name) : base(texture, color, name)
        {
            this.box = box;
            innerRenderLimit = box.GetInflated(-TransitionStart);
            outerRenderLimit = box.GetInflated(TransitionStart);

            Vector3D origin = box.Min;
            Vector3D size = box.Size;

            faces[0] = new BoxFace(origin, size, Base6Directions.Direction.Up, TransitionStart, MaterialId, color);
            faces[1] = new BoxFace(origin, size, Base6Directions.Direction.Down, TransitionStart, MaterialId, color);
            faces[2] = new BoxFace(origin, size, Base6Directions.Direction.Left, TransitionStart, MaterialId, color);
            faces[3] = new BoxFace(origin, size, Base6Directions.Direction.Right, TransitionStart, MaterialId, color);
            faces[4] = new BoxFace(origin, size, Base6Directions.Direction.Forward, TransitionStart, MaterialId, color);
            faces[5] = new BoxFace(origin, size, Base6Directions.Direction.Backward, TransitionStart, MaterialId, color);
        }


        public override bool Draw(Vector3D camera)
        {
            if (IsOutsideRenderDistance(camera))
                return false;

            double dist2 = box.DistanceSquared(ref camera);

            foreach (BoxFace face in faces)
                face.Draw(camera);
            return true;
        }

        private bool IsOutsideRenderDistance(Vector3D camera)
        {
            return outerRenderLimit.Contains(camera) != ContainmentType.Contains || innerRenderLimit.Contains(camera) == ContainmentType.Contains;
        }

        public double GetDistance(Vector3D position)
        {
            if (box.Contains(position) != ContainmentType.Contains)
            {
                Vector3D surfacePosition = Vector3D.Clamp(position, box.Min, box.Max);
                return Math.Abs(Vector3D.Distance(position, surfacePosition));
            }

            Vector3D perAxisMin = Vector3D.Min(Vector3D.Abs(position - box.Min), Vector3D.Abs(box.Max - position));
            return perAxisMin.Min();
        }

        public override bool IsInside(Vector3D position)
        {
            ContainmentType type = box.Contains(position);
            return type == ContainmentType.Contains;
        }


        public override bool Raycast(RayD ray, out double distance)
        {
            distance = double.PositiveInfinity;

            if (box.Contains(ray.Position) != ContainmentType.Contains)
            {
                double? outResult = ray.Intersects(box);
                if(!outResult.HasValue)
                    return false;

                distance = outResult.Value;
                return true;
            }

            foreach(BoxFace face in faces)
            {
                if (face.Raycast(ray, out distance))
                    return true;
            }
            return false;
        }

        private class BoxFace
        {
            private readonly Vector3D min;
            private readonly Vector3D max;
            private readonly Base6Directions.Direction direction;
            private readonly double maximumDistance;
            private readonly MyStringId material;
            private readonly int numTextures;
            private readonly Vector3D normal;

            private readonly Vector3D planeMask;

            private readonly Vector3D dirX;
            private readonly Vector3D dirY;

            private readonly Vector4 color = Vector4.One;

            public BoxFace(Vector3D boxMin, Vector3D boxSize, Base6Directions.Direction direction, double transitionStart, MyStringId material, Color color)
            {
                this.color = color.ToVector4();
                this.direction = direction;
                this.maximumDistance = transitionStart;
                this.material = material;
                numTextures = (int)Math.Max(transitionStart / BorderTextureSize, 1);
                normal = (Vector3D)Base6Directions.GetVector(direction);
                min = boxMin;
                max = boxMin + boxSize;

                switch (direction)
                {
                    case Base6Directions.Direction.Forward:
                        max.Z = min.Z;
                        dirX = Vector3D.Right;
                        dirY = Vector3D.Up;
                        break;
                    case Base6Directions.Direction.Backward:
                        min.Z = max.Z;
                        dirX = Vector3D.Right;
                        dirY = Vector3D.Up;
                        break;
                    case Base6Directions.Direction.Left:
                        max.X = min.X;
                        dirX = Vector3D.Backward;
                        dirY = Vector3D.Up;
                        break;
                    case Base6Directions.Direction.Right:
                        min.X = max.X;
                        dirX = Vector3D.Backward;
                        dirY = Vector3D.Up;
                        break;
                    case Base6Directions.Direction.Up:
                        min.Y = max.Y;
                        dirX = Vector3D.Right;
                        dirY = Vector3D.Backward;
                        break;
                    case Base6Directions.Direction.Down:
                        max.Y = min.Y;
                        dirX = Vector3D.Right;
                        dirY = Vector3D.Backward;
                        break;
                }

                planeMask = Vector3D.Abs(dirX + dirY);
            }

            public void Draw(Vector3D camera)
            {
                double dist = GetDistance(camera);
                if(dist > maximumDistance)
                    return;

                ClampToGrid(ref camera);

                for (int x = -numTextures; x <= numTextures; x++)
                {
                    Vector3D xComp = dirX * (x * BorderTextureSize);
                    for (int y = -numTextures; y <= numTextures; y++)
                    {
                        Vector3D coords = camera + xComp + (dirY * (y * BorderTextureSize));
                        if (IsTextureInBounds(coords))
                            DrawTexture(coords);
                    }
                }
            }

            public bool Raycast(RayD ray, out double distance)
            {
                distance = double.PositiveInfinity;

                // https://en.wikipedia.org/wiki/Line%E2%80%93plane_intersection

                double directionDotNormal = ray.Direction.Dot(normal);
                if (directionDotNormal == 0)
                    return false;

                Vector3D lineToPlane = min - ray.Position;
                double positionDotNormal = lineToPlane.Dot(normal);

                double result = positionDotNormal / directionDotNormal;
                if (result < 0)
                    return false;

                Vector3D position = ray.Position + (ray.Direction * result);
                if(IsInBounds(position))
                {
                    distance = result;
                    return true;
                }
                return false;
            }

            public double GetDistance(Vector3D position)
            {
                return Math.Abs(VectorTools.ScalarProjection(position - min, normal));
            }

            private void DrawTexture(Vector3D position)
            {
                MyTransparentGeometry.AddBillboardOriented(material, color, position, dirX, dirY, BorderTextureRadius, BorderTextureRadius);
            }

            private bool IsTextureInBounds(Vector3D position)
            {
                return IsInBounds(position - (BorderTextureRadius * planeMask)) && IsInBounds(position + (BorderTextureRadius * planeMask));
            }

            private bool IsInBounds(Vector3D position)
            {
                switch (direction)
                {
                    case Base6Directions.Direction.Forward:
                    case Base6Directions.Direction.Backward:
                        return position.X >= min.X && position.X <= max.X
                            && position.Y >= min.Y && position.Y <= max.Y;
                    case Base6Directions.Direction.Left:
                    case Base6Directions.Direction.Right:
                        return position.Y >= min.Y && position.Y <= max.Y
                            && position.Z >= min.Z && position.Z <= max.Z;
                    default:
                        return position.X >= min.X && position.X <= max.X
                            && position.Z >= min.Z && position.Z <= max.Z;
                }
            }

            private void ClampToGrid(ref Vector3D value)
            {
                value = Vector3D.Round(value / (BorderTextureRadius * 2)) * (BorderTextureRadius * 2);

                switch (direction)
                {
                    case Base6Directions.Direction.Forward:
                    case Base6Directions.Direction.Backward:
                        value.Z = min.Z;
                        break;
                    case Base6Directions.Direction.Left:
                    case Base6Directions.Direction.Right:
                        value.X = min.X;
                        break;
                    default:
                        value.Y = min.Y;
                        break;
                }
            }

            
        }
    }
}
