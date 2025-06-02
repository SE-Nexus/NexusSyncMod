using Draygo.API;
using NexusSyncMod.Players;
using NexusSyncMod.Respawn;
using Sandbox.Engine.Multiplayer;
using Sandbox.Engine.Networking;
using Sandbox.Game;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI;
using VRage.GameServices;
using VRageMath;

namespace NexusSyncMod.Data.Scripts.NexusSyncMod.Players
{
    internal class PlayerStatusHud
    {
        public const ushort NETWORK_ID = 2939;

        private StringBuilder hudMsgText = new StringBuilder();
        private HudAPIv2.HUDMessage hudMsg;

        private ScreenManagerEvent onScreenAdded;
        private ScreenManagerEvent onScreenRemoved;

        private bool visible = true;

        public void Init()
        {
            MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(NETWORK_ID, MessageHandler);
        }


        private void MessageHandler(ushort id, byte[] data, ulong sender, bool fromServer)
        {
            string recievedMessage = MyAPIGateway.Utilities.SerializeFromBinary<string>(data);

            try
            {
                if (ModCore.DEBUG)
                    MyAPIGateway.Utilities.ShowNotification($"Received Lobby Data: {recievedMessage}", 1000);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to parse recievedMessage");
                if (ModCore.DEBUG)
                {
                    hudMsgText.Clear();
                    hudMsgText.Append($"Parse fail! {ex.Message}").AppendLine();
                    Center(hudMsg);
                }
                return;
            }
            UpdateLobbyData(recievedMessage);

        }

        private void UpdateLobbyData(string recievedMessage)
        {
            hudMsgText.Clear();
            hudMsgText.Append(recievedMessage).AppendLine();
            Center(hudMsg);

            if (ModCore.DEBUG)
                MyAPIGateway.Utilities.ShowNotification($"Received Lobby Data:  {recievedMessage}", 1000);
        }
        private void Center(HudAPIv2.HUDMessage msg)
        {
            double length = msg.GetTextLength().X;
            msg.Offset = new Vector2D(-length / 2, msg.Offset.Y);
        }
        public void InitHud()
        {
            if (ModCore.DEBUG) 
                hudMsgText.AppendLine("Lobby messages have been initialized.").AppendLine();
            hudMsg = new HudAPIv2.HUDMessage(hudMsgText, new Vector2D(0.0, 0.8), HideHud: false)
            {
                Visible = visible,
            };
            hudMsg.Scale = 1.5f;
            Center(hudMsg);
        }

        public void Unload()
        {
            MyAPIGateway.Multiplayer.UnregisterSecureMessageHandler(NETWORK_ID, MessageHandler);
            if (onScreenAdded != null)
            {
                MyVisualScriptLogicProvider.ScreenAdded -= onScreenAdded;
                MyVisualScriptLogicProvider.ScreenRemoved -= onScreenRemoved;
            }

            MyAPIGateway.Gui.GuiControlCreated -= Gui_GuiControlCreated;
            MyAPIGateway.Gui.GuiControlRemoved -= Gui_GuiControlRemoved;
        }

        private void Gui_GuiControlCreated(object obj)
        {
            OnScreenAdded(obj.GetType().Name);
        }

        private void Gui_GuiControlRemoved(object obj)
        {
            OnScreenRemoved(obj.GetType().Name);
        }

        private void OnScreenAdded(string screen)
        {
            visible = true;
        }

        public void OnScreenRemoved(string screen)
        {
            visible = false;
        }
    }
}
