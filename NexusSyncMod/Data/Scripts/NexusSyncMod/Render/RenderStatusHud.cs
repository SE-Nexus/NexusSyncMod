using Draygo.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace NexusSyncMod.Render
{
    internal class RenderStatusHud
    {
        private StringBuilder titleBuilder = new StringBuilder("Nexus Border Renderer");
        private HudAPIv2.HUDMessage title;

        private StringBuilder distanceBuilder = new StringBuilder("999km - 0:00 ");
        private HudAPIv2.HUDMessage distance;

        private string name;
        private bool entering;

        private bool visible = true;
        public bool Visible 
        {
            get 
            {
                return visible;
            }
            set 
            {
                if(value != visible)
                {
                    title.Visible = value;
                    distance.Visible = value;
                    visible = value;
                }
            }
        }

        public RenderStatusHud(Color color) 
        {
            title = new HudAPIv2.HUDMessage(titleBuilder, new Vector2D(0, 0.95), Font: "white", Scale: 2)
            {
                Visible = true,
                InitialColor = color,
            };
            double verticalOffset = title.GetTextLength().Y;
            Center(title);

            distance = new HudAPIv2.HUDMessage(distanceBuilder, new Vector2D(0, title.Origin.Y + verticalOffset), Font: "monospace")
            {
                Visible = true,
                InitialColor = color,
            };
            Center(distance);
        }

        private void Center(HudAPIv2.HUDMessage msg)
        {
            double length = msg.GetTextLength().X;
            msg.Offset = new Vector2D(-length / 2, msg.Offset.Y);
        }

        public void Update(double distance, double seconds, string name, bool isInside)
        {
            Visible = true;

            if(this.entering != isInside || this.name != name)
            {
                titleBuilder.Clear();
                if (isInside)
                    titleBuilder.Append("Exiting ");
                else
                    titleBuilder.Append("Entering ");
                titleBuilder.Append(name);
                Center(title);

                this.name = name;
                this.entering = isInside;
            }

            distanceBuilder.Clear();

            if(distance < 1000)
                distanceBuilder.AppendFormat("{0,4}", (int)distance).Append("m");
            else
                distanceBuilder.AppendFormat("{0:0.0}", distance / 1000).Append("km");

            distanceBuilder.Append(" - ");

            int secondsRounded = (int)(seconds + 0.5);
            if(seconds >= 60)
            {
                int secondsRemaining;
                int minutesRemaining = Math.DivRem(secondsRounded, 60, out secondsRemaining);
                distanceBuilder.Append(minutesRemaining);
                if (secondsRemaining < 10)
                    distanceBuilder.Append(":0").Append(secondsRemaining);
                else
                    distanceBuilder.Append(':').Append(secondsRemaining);
            }
            else
            {
                distanceBuilder.Append(secondsRounded);
            }
        }
    }
}
