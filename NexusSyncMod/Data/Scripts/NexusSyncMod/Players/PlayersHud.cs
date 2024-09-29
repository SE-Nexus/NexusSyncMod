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
                visible = value;
                if (hudMsg != null)
                    hudMsg.Visible = value;
            }
        }

        private bool init;

        public void Init()
        {
            // Hack to bypass the whitelist (this event has an unsupported type)
            onScreenAdded = (o) => OnScreenAdded(o.GetType().Name);
            onScreenRemoved = (o) => OnScreenRemoved(o.GetType().Name);
            MyVisualScriptLogicProvider.ScreenAdded += onScreenAdded;
            MyVisualScriptLogicProvider.ScreenRemoved += onScreenRemoved;

            MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(SeamlessClientNetId, MessageReceived);
        }

        private void MessageReceived(ushort id, byte[] data, ulong sender, bool fromServer)
        {
            if (!fromServer)
                return;


            OnlinePlayerData playerData;
            try
            {
                playerData = MyAPIGateway.Utilities.SerializeFromBinary<OnlinePlayerData>(data);
            }
            catch
            {
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
            hudMsgText.Append("Players in Cluster:").AppendLine();
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
            if(onScreenAdded != null)
            {
                MyVisualScriptLogicProvider.ScreenAdded -= onScreenAdded;
                MyVisualScriptLogicProvider.ScreenRemoved -= onScreenRemoved;
            }
        }

        public void Update()
        {
            if (ModCore.DEBUG && !init)
            {
                init = true;
                UpdatePlayerList(new OnlinePlayerData()
                {
                    /*OnlineServers = new List<OnlineClientServer>()
                    {
                        new OnlineClientServer()
                        {
                            Players = new List<OnlinePlayer>()
                            {
                                new OnlinePlayer("avaness", 76561198082681546, 28847760, 13),
                                new OnlinePlayer("Casimir", 76561198045096439, 59452696, 13),
                                new OnlinePlayer("BishyBash", 76561198045096431, 59452696, 13),
                                new OnlinePlayer("VastMan", 76561198045096431, 59452696, 13),
                            }
                        }
                    }*/
                });
            }
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
