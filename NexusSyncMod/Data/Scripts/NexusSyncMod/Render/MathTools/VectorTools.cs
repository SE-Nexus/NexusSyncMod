using System;
using VRageMath;

namespace NexusSyncMod.Render.Shapes
{
    internal static class VectorTools
    {
        public static double GetWorldAzimuth(Vector3D vector)
        {
            vector.Y = 0.0;
            vector.Normalize();
            double dotForward;
            Vector3D.Dot(ref vector, ref Vector3D.Forward, out dotForward);
            if (vector.X >= 0.0)
                return -Math.Acos(dotForward);
            return Math.Acos(dotForward);
        }

        public static double GetWorldElevation(Vector3D vector)
        {
            double result;
            Vector3D.Dot(ref vector, ref Vector3D.Up, out result);
            return Math.Asin(result);
        }

        public static double GetElevation(Vector3D vector, Vector3D up)
        {
            double result;
            Vector3D.Dot(ref vector, ref up, out result);
            return Math.Asin(result);
        }

        public static double GetAzimuth(Vector3D vector, MatrixD matrix)
        {
            double dotUp = Vector3D.Dot(vector, matrix.Up);
            vector -= dotUp * matrix.Up;
            vector.Normalize();
            double dotForward = Vector3D.Dot(vector, matrix.Forward);
            double azimuth = Math.Acos(dotForward);
            if (Vector3D.Dot(vector, matrix.Right) > 0)
                azimuth *= -1;
            return azimuth;
        }

        public static void GetAzimuthElevation(Vector3D vector, MatrixD matrix, out double azimuth, out double elevation)
        {
            double dotUp = Vector3D.Dot(vector, matrix.Up);
            elevation = Math.Asin(dotUp);
            vector -= dotUp * matrix.Up;
            vector.Normalize();
            double dotForward = Vector3D.Dot(vector, matrix.Forward);
            azimuth = Math.Acos(dotForward);
            if (Vector3D.Dot(vector, matrix.Right) > 0)
                azimuth *= -1;
        }


        public static double MakeAngleLoopable(double angle)
        {
            return (Math.PI * 2) / Math.Ceiling((Math.PI * 2) / angle);
        }

        /// <summary>
        /// Projects a value onto another vector.
        /// </summary>
        /// <param name="guide">Must be of length 1.</param>
        public static double ScalarProjection(Vector3D value, Vector3D guide)
        {
            double returnValue = Vector3.Dot(value, guide);
            if (double.IsNaN(returnValue))
                return 0;
            return returnValue;
        }

        /// <summary>
        /// Projects a value onto another vector.
        /// </summary>
        /// <param name="guide">Must be of length 1.</param>
        public static Vector3D VectorProjection(Vector3D value, Vector3D guide)
        {
            return ScalarProjection(value, guide) * guide;
        }

        /// <summary>
        /// Projects a value onto another vector.
        /// </summary>
        /// <param name="guide">Must be of length 1.</param>
        public static Vector3D VectorRejection(Vector3D value, Vector3D guide)
        {
            return value - VectorProjection(value, guide);
        }

        public static void ClampToGrid(ref double angle, double grid)
        {
            angle = Math.Round(angle / grid) * grid;
        }

        public static Vector3D WorldToLocalPosition(Vector3D worldPosition, Vector3D referencePosition, MatrixD referenceTransposed)
        {
            return Vector3D.TransformNormal(worldPosition - referencePosition, referenceTransposed);
        }

        public static Vector3D WorldToLocalDirection(Vector3D worldDirection, MatrixD referenceTransposed)
        {
            return Vector3D.TransformNormal(worldDirection, referenceTransposed);
        }

        public static Vector3D LocalToWorldPosition(Vector3D localPosition, MatrixD reference)
        {
            return Vector3D.Transform(localPosition, reference);
        }

        public static Vector3D LocalToWorldDirection(Vector3D localDirection, MatrixD reference)
        {
            return Vector3D.TransformNormal(localDirection, reference);
        }
    }
}
