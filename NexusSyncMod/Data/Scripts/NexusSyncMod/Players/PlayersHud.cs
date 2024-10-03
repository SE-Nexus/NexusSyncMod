using Draygo.API;
using Sandbox.Game;
using Sandbox.ModAPI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage.Game.ModAPI;
using VRageMath;

namespace NexusSyncMod.Players
{
    internal class PlayersHud
    {
        private const string ScreenName = "MyGuiScreenPlayers";
        public const ushort SeamlessClientNetId = 2936;

        private StringBuilder hudMsgText = new StringBuilder();
        private HudAPIv2.HUDMessage hudMsg;

        private ScreenManagerEvent onScreenAdded;
        private ScreenManagerEvent onScreenRemoved;

        private bool visible;
        private bool Visible 
        {
            get 
            {
                return visible;
            }
            set 
            {
                if(visible != value)
                {
                    visible = value;
                    if (hudMsg != null)
                        hudMsg.Visible = value;
                    if (ModCore.DEBUG)
                        Log.Info("Players visible: " + value);
                }
            }
        }

        public void Init()
        {
            // Method 1 to detect screens: 
            // triggered when "control"? added/removed
            MyAPIGateway.Gui.GuiControlCreated += Gui_GuiControlCreated;
            MyAPIGateway.Gui.GuiControlRemoved += Gui_GuiControlRemoved;

            // Method 2 to detect screens:
            // MyVisualScriptLogicProvider events, inconsistent due to bug in multiplayer
            onScreenAdded = (o) => OnScreenAdded(o.GetType().Name);
            onScreenRemoved = (o) => OnScreenRemoved(o.GetType().Name);
            MyVisualScriptLogicProvider.ScreenAdded += onScreenAdded;
            MyVisualScriptLogicProvider.ScreenRemoved += onScreenRemoved;

            MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(SeamlessClientNetId, MessageReceived);
        }


        private void MessageReceived(ushort id, byte[] data, ulong sender, bool fromServer)
        {
            if (!fromServer)
            {
                Log.Error("Received invalid players packet");
                return;
            }


            OnlinePlayerData playerData;
            try
            {
                playerData = MyAPIGateway.Utilities.SerializeFromBinary<OnlinePlayerData>(data);
                if (ModCore.DEBUG)
                    MyAPIGateway.Utilities.ShowNotification($"Received {playerData?.OnlinePlayers?.Count ?? 0}", 1000);
            }
            catch
            {
                Log.Error("Failed to parse OnlinePlayerData");
                return;
            }
            UpdatePlayerList(playerData);
            
        }

        private void UpdatePlayerList(OnlinePlayerData playerData) 
        {
            List<IMyPlayer> players = new List<IMyPlayer>();
            MyAPIGateway.Players.GetPlayers(players);
            HashSet<ulong> playerIds = new HashSet<ulong>(players.Where(x => !x.IsBot).Select(x => x.SteamUserId));

            hudMsgText.Clear();
            hudMsgText.Append("Other online players:").AppendLine();
            int added = 0;
            foreach (OnlinePlayer player in playerData.OnlinePlayers)
            {
                if (!playerIds.Contains(player.SteamID))
                {
                    hudMsgText.Append(player.PlayerName).AppendLine();
                    added++;
                }
            }

            if(added <= 0)
                hudMsgText.Clear();
            if(ModCore.DEBUG)
                MyAPIGateway.Utilities.ShowNotification($"Added {added}", 1000);
        }

        public void InitHud()
        {
            hudMsg = new HudAPIv2.HUDMessage(hudMsgText, new Vector2D(0.65, 0.8), HideHud: false)
            {
                Visible = visible,
            };
        }

        public void Unload()
        {
            MyAPIGateway.Multiplayer.UnregisterSecureMessageHandler(SeamlessClientNetId, MessageReceived);
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
            if(screen == ScreenName)
                Visible = true;
        }

        public void OnScreenRemoved(string screen)
        {
            if (screen == ScreenName)
                Visible = false;
        }
    }
}
