using NexusSyncMod.Render.MathTools;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using VRageMath;

namespace NexusSyncMod.Render.Shapes
{
    internal class TorusBorder : BorderShape
    {
        private readonly QuarticSolver quarticSolver = new QuarticSolver();

        private readonly int numTextures;
        private readonly MatrixD matrix;
        private readonly MatrixD matrixTransposed;
        private readonly double maxDistance2;
        private readonly double maxRingDistance2;
        private readonly double majorRadius;
        private readonly double minorRadius;

        public TorusBorder(Vector3D center, Vector3D up, double majorRadius, double minorRadius, string texture, Color color, string name) : base(texture, color, name)
        {
            numTextures = (int)Math.Max(TransitionStart / BorderTextureSize, 1);

            matrix = MatrixD.CreateWorld(center, Vector3D.CalculatePerpendicularVector(up), up);
            matrixTransposed = MatrixD.Transpose(matrix);

            maxDistance2 = majorRadius + minorRadius + TransitionStart;
            maxDistance2 *= maxDistance2;

            maxRingDistance2 = minorRadius + TransitionStart;
            maxRingDistance2 *= maxRingDistance2;

            this.majorRadius = majorRadius;
            this.minorRadius = minorRadius;
        }

        public override bool Draw(Vector3D camera)
        {
            Vector3D worldCameraDir = camera - matrix.Translation;
            double dist2 = worldCameraDir.LengthSquared();
            if (dist2 > maxDistance2)
                return false;

            Vector3D onPlaneDir = Vector3D.Normalize(VectorTools.VectorRejection(worldCameraDir, matrix.Up));

            Vector3D minorCircleCenter = matrix.Translation + onPlaneDir * majorRadius;

            Vector3D toSurfaceDir = camera - minorCircleCenter;
            double distanceToSurface2 = toSurfaceDir.LengthSquared();
            if (distanceToSurface2 > maxRingDistance2)
                return false;
            toSurfaceDir /= Math.Sqrt(distanceToSurface2);

            double elevationStep = VectorTools.MakeAngleLoopable(SizeToRadians(BorderTextureSize, minorRadius));

            double currentEl = VectorTools.GetElevation(toSurfaceDir, matrix.Up);
            if (toSurfaceDir.Dot(onPlaneDir) < 0)
            {
                if (currentEl >= 0)
                    currentEl = Pi - currentEl;
                else
                    currentEl = -(Pi + currentEl);
            }
            double currentAz = VectorTools.GetAzimuth(onPlaneDir, matrix);

            Vector3D localPos = new Vector3D();
            Vector3D localCenter = new Vector3D();
            for (int e = -numTextures; e <= numTextures; e++)
            {
                double elevation = e * elevationStep;
                if (Math.Abs(elevation) > Pi || elevation == Pi)
                    continue; // Rendering the entire minor circle of the torus
                elevation += currentEl;
                if (Math.Abs(elevation) > Pi)
                    elevation -= Pi2 * Math.Sign(elevation);
                VectorTools.ClampToGrid(ref elevation, elevationStep);
                double currentMajorRadius = GetRadius(elevation);
                localPos.Y = Math.Sin(elevation) * minorRadius;
                
                double azimuthStep = VectorTools.MakeAngleLoopable(SizeToRadians(BorderTextureSize, currentMajorRadius));
                for (int a = -numTextures; a <= numTextures; a++)
                {
                    double azimuth = a * azimuthStep;
                    if (Math.Abs(azimuth) > Pi)
                        continue; // Rendering the entire torus
                    azimuth += currentAz + Pi / 2;
                    if (Math.Abs(azimuth) > Pi)
                        azimuth -= Pi2 * Math.Sign(azimuth);
                    VectorTools.ClampToGrid(ref azimuth, azimuthStep);
                    localPos.X = Math.Cos(azimuth);
                    localPos.Z = -Math.Sin(azimuth);
                    
                    localCenter.X = localPos.X * majorRadius;
                    localCenter.Z = localPos.Z * majorRadius;
                    localPos.X *= currentMajorRadius;
                    localPos.Z *= currentMajorRadius;
                    Vector3D toSurface = Vector3D.Normalize(localPos - localCenter);
                    
                    DrawTextureLocal(localPos, toSurface);
                }
            }

            return true;
        }
        private double GetRadius(double elevation)
        {
            return (Math.Cos(elevation) * minorRadius) + majorRadius;
        }

        private void DrawTextureLocal(Vector3D localPos, Vector3D localNormal)
        {
            Vector3D position = Vector3D.Transform(localPos, matrix);
            Vector3D normal = Vector3D.TransformNormal(localNormal, matrix);
            DrawTexture(position, normal);
        }

        private double SizeToRadians(double size, double radius)
        {
            return size / radius;
        }

        private Vector3D GetVectorFromRingCenter(Vector3D position)
        {
            // Project the position onto the plane formed by the direction vector
            Vector3D fromCenter = position - matrix.Translation;
            Vector3D projectedPosition = VectorTools.VectorRejection(fromCenter, matrix.Up);

            // Use the direction on the plane to calculate the closest ring center
            Vector3D ringCenter = matrix.Translation + (projectedPosition.Normalized() * majorRadius);

            return position - ringCenter;
        }

        public override bool IsInside(Vector3D position)
        {
            Vector3D center = matrix.Translation;
            Vector3D direction = matrix.Up;

            //Simple check if inside the two spheres
            double centerDistance = Vector3D.Distance(position, center);
            if (centerDistance > (majorRadius + minorRadius) || centerDistance < majorRadius - minorRadius)
                return false;

            double centerDist2 = GetVectorFromRingCenter(position).LengthSquared();
            return centerDist2 < (minorRadius * minorRadius);
        }

        private double GetDistanceFromTorus(Vector3D position)
        {
            double distFromRingCenter = GetVectorFromRingCenter(position).Length();
            return Math.Abs(distFromRingCenter - minorRadius);
        }

        #region Raycast
        /* Original c++ source from:
         * http://cosinekitty.com/raytrace/source.html
         * 
         * References:
         * http://cosinekitty.com/raytrace/chapter13_torus.html
         * https://johannes.stoerkle.info/wp-content/uploads/2020StoerkleJohannes_TechnicalNotes_TorodialSurface_MathFormulation_Optics.pdf
         * https://math.stackexchange.com/a/786
         * https://en.wikipedia.org/wiki/Quartic_equation
         * 
         * Original copyright notice:
            torus.cpp, algebra.h, algebra.cpp

            Copyright (C) 2013 by Don Cross  -  http://cosinekitty.com/raytrace

            This software is provided 'as-is', without any express or implied
            warranty. In no event will the author be held liable for any damages
            arising from the use of this software.

            Permission is granted to anyone to use this software for any purpose,
            including commercial applications, and to alter it and redistribute it
            freely, subject to the following restrictions:

            1. The origin of this software must not be misrepresented; you must not
               claim that you wrote the original software. If you use this software
               in a product, an acknowledgment in the product documentation would be
               appreciated but is not required.

            2. Altered source versions must be plainly marked as such, and must not be
               misrepresented as being the original software.

            3. This notice may not be removed or altered from any source
               distribution.
        */
        public override bool Raycast(RayD ray, out double distance)
        {
            Vector3D localPosition = VectorTools.WorldToLocalPosition(ray.Position, matrix.Translation, matrixTransposed);
            Vector3D localDirection = VectorTools.WorldToLocalDirection(ray.Direction, matrixTransposed);

            if(!TrySolveIntersections(localPosition, localDirection, out distance))
            {
                distance = double.PositiveInfinity;
                return false;
            }
            return true;
        }

        private bool TrySolveIntersections(Vector3D vantage, Vector3D direction, out double minReal)
        {
            double R = majorRadius;
            double S = minorRadius;

            // Set up the coefficients of a quartic equation for the intersection
            // of the parametric equation P = vantage + u*direction and the 
            // surface of the torus.
            // There is far too much algebra to explain here.
            // See the text of the tutorial for derivation.

            double T = 4.0 * R * R;
            double G = T * (direction.X * direction.X + direction.Z * direction.Z);
            double H = 2.0 * T * (vantage.X * direction.X + vantage.Z * direction.Z);
            double I = T * (vantage.X * vantage.X + vantage.Z * vantage.Z);
            double J = direction.LengthSquared();
            double K = 2.0 * vantage.Dot(direction);
            double L = vantage.LengthSquared() + R * R - S * S;

            IEnumerable<double> results = quarticSolver.SolveQuartic(
                J * J,                    // coefficient of u^4
                2.0 * J * K,                // coefficient of u^3
                2.0 * J * L + K * K - G,      // coefficient of u^2
                2.0 * K * L - H,            // coefficient of u^1 = u
                L * L - I                // coefficient of u^0 = constant term
            ).Where(x => x > 0);

            if(results.Any())
            {
                minReal = results.Min();
                return true;
            }

            minReal = double.NaN;
            return false;
        }

        #endregion
    }
}
