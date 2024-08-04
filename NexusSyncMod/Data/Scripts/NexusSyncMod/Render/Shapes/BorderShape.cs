using Sandbox.Definitions;
using System;
using System.Linq;
using VRage.Game;
using VRage.Utils;
using VRageMath;

namespace NexusSyncMod.Render.Shapes
{
    internal abstract class BorderShape
    {
        protected const double Pi = Math.PI;
        protected const double Pi2 = Pi * 2;

        protected const float BorderTextureRadius = 100;
        protected const float BorderTextureSize = BorderTextureRadius * 2;

        protected readonly MyStringId MaterialId;
        protected readonly double TransitionStart;
        protected readonly double TransitionEnd;
        private readonly Vector4 color;

        public string Name { get; }

        public BorderShape(string material, Color color, string name)
        {
            MaterialId = MyStringId.GetOrCompute(material);
            this.color = color.ToVector4();

            MyTransparentMaterialDefinition def = MyDefinitionManager.Static.GetTransparentMaterialDefinitions().FirstOrDefault(x => x.Id.SubtypeName == material);
            if (def != null)
            {
                TransitionStart = def.AlphaMistingStart;
                TransitionEnd = def.AlphaMistingEnd;
            }

            if (TransitionStart <= 0)
                TransitionStart = 2000;

            if (TransitionEnd <= 0)
                TransitionEnd = 1000;
            Name = name;
        }

        public abstract bool Draw(Vector3D camera);

        protected void DrawTexture(Vector3D position, Vector3D normal)
        {
            Vector3D axis1 = normal.Cross(Vector3D.Up).Normalized();
            Vector3D axis2 = normal.Cross(axis1).Normalized();

            MyTransparentGeometry.AddBillboardOriented(MaterialId, color, position, axis1, axis2, BorderTextureRadius, BorderTextureRadius);

        }


        public abstract bool IsInside(Vector3D position);

        public abstract bool Raycast(RayD ray, out double distance);
    }
}
