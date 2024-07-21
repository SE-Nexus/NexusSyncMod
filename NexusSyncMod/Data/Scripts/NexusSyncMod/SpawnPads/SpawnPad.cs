using NexusSyncMod.Nexus;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;

namespace NexusSyncMod.SpawnPads
{
    public class SpawnPad
    {
        private const string Font = "White";
        private const int DissapearTime = 1000 - 25;
        private const int CountdownTimer = 3;
        private static readonly Regex RegCustomData = new Regex(":(.*)");
        private static readonly Guid StorageGUID = new Guid("9416E3EB-216D-493D-914D-98AA90E88FB1");


        private enum SpawnType
        {
            Single,
            Multi
        }

        private SpawnType type;
        private MyEntity entity;
        private IMyCubeBlock block;
        private IMyTerminalBlock terminalBlock;
        private IMyRadioAntenna radioAnt;
        private string subType;
        private int currentTimer = 4;

        private double maxDistance = 1.5;

        private SpawnPadConfigs configs = new SpawnPadConfigs();

        private List<IMyPlayer> containedPlayers = new List<IMyPlayer>();


        public SpawnPad(MyEntity entity)
        {
            block = (IMyCubeBlock)entity;
            terminalBlock = (IMyTerminalBlock)block;
            radioAnt = (IMyRadioAntenna)block;

            subType = block.BlockDefinition.SubtypeId;

            if (subType == "SpawnPadSingle")
            {
                type = SpawnType.Single;
                maxDistance = 1.5f;
            }
            else if (subType == "SpawnPadMulti")
            {
                type = SpawnType.Multi;
                maxDistance = 2.5;
            }

            this.entity = entity;

            //If this doesnt have a storage component, go ahead and add one
            if (!LoadData())
                SaveData();

            AddCustomDataConfigs();
            //RadioAnt.CustomDataChanged += RadioAnt_CustomDataChanged;

        }

        public void Update()
        {

            //If the configs failed to recieve, skip update
            if (!GetCustomDataSettings())
                return;


            AccumulatePlayers();
            CheckStatus();


        }


        private IEnumerable<IMyCharacter> GetCharactersInBlock()
        {
            Vector3D blockPosition = block.PositionComp.GetPosition();
            IEnumerable<IMyCharacter> characters = MyEntities.GetEntities().OfType<IMyCharacter>();

            foreach (IMyCharacter character in characters)
            {

                if (!character.InScene || character.IsBot || character.IsDead || !character.IsPlayer || string.IsNullOrEmpty(character.DisplayName))
                    continue;

                if (Vector3D.Distance(character.GetPosition(), blockPosition) < maxDistance)
                    yield return (character);
            }
        }

        private void AccumulatePlayers()
        {
            bool passedNewTimerCheck = true;

            // Accumulate all players in the zone
            List<IMyPlayer> containedPlayers = new List<IMyPlayer>();
            foreach (IMyCharacter character in GetCharactersInBlock())
            {
                if (character == null)
                    continue;

                IMyPlayer myPlayer = MyAPIGateway.Players.GetPlayerControllingEntity(character);

                if (myPlayer == null)
                    continue;

                containedPlayers.Add(myPlayer);

                if (!this.containedPlayers.Contains(myPlayer))
                {
                    //Reset the countdown timer if a new player joins
                    this.containedPlayers.Add(myPlayer);
                    currentTimer = CountdownTimer;
                    passedNewTimerCheck = false;
                    continue;
                }
            }

            if (containedPlayers.Count == 0)
            {
                this.containedPlayers.Clear();
                return;
            }


            // Remove any players in the dictionary that no longer belong
            for (int i = this.containedPlayers.Count - 1; i >= 0; i--)
            {
                if (!containedPlayers.Contains(this.containedPlayers[i]))
                {
                    // Reset the current timer if a player left
                    currentTimer = CountdownTimer;
                    passedNewTimerCheck = false;
                    this.containedPlayers.RemoveAt(i);
                }
            }

            if (passedNewTimerCheck)
                currentTimer--;
        }

        private void CheckStatus()
        {
            if (containedPlayers.Count == 0)
                return;

            //Check to see if pad is enabled and working
            if (!radioAnt.Enabled || !radioAnt.IsWorking)
            {
                BroadcastMessage("SpawnPad is disabled or non-functional!");
                return;
            }


            //Check to see if the target server is online
            if (configs.TargetServerID > 0 && !NexusAPI.IsServerOnline(configs.TargetServerID))
            {
                BroadcastMessage("Target server is not online!");
                return;
            }


            // Check to see if the target server has room
            if (!HasRoom(configs.TargetServerID))
                return;


            // Begin checks for either single or multi spawn pad
            if (type == SpawnType.Single)
            {
                if (containedPlayers.Count > 1)
                {
                    BroadcastMessage("Only one player per pad!");
                    return;
                }
            }
            else if (type == SpawnType.Multi)
            {
                if (configs.MaxPlayers != 0 && containedPlayers.Count > configs.MaxPlayers)
                {
                    BroadcastMessage($"Max of {configs.MaxPlayers} players on this pad!");
                    return;
                }

                if (configs.MinPlayers != 0 && containedPlayers.Count < configs.MinPlayers)
                {
                    BroadcastMessage($"You need {configs.MinPlayers - containedPlayers.Count} more players to use this pad!");
                    return;
                }
            }

            CheckSingleStatus();
        }

        private bool HasRoom(int targetServerId)
        {

            if (targetServerId <= 0)
                return true;

            int totalOnlinePlayers = NexusAPI.GetAllOnlinePlayers().Count(x => x.OnServer == targetServerId);

            Server targetServer = NexusAPI.GetAllOnlineServers().FirstOrDefault(x => x.ServerID == targetServerId);
            if (targetServer == null)
                return false;


            //Check max players
            if (targetServer.MaxPlayers < totalOnlinePlayers + containedPlayers.Count)
            {
                // Server is full, but the server can still fit the players if they are reserved

                bool areReservedPlayers = containedPlayers.All(x => targetServer.ReservedPlayers.Contains(x.SteamUserId));
                if(!areReservedPlayers)
                {
                    BroadcastMessage("Target server is full");
                    return false;
                }
            }


            return true;
        }


        private void CheckSingleStatus()
        {
            //Check each players status in the configs
            string message = "";
            bool passedChecks = true;


            foreach (IMyPlayer player in containedPlayers)
            {
                if (player == null)
                    continue;

                PlayerPadUse use = configs.GetPlayerFromPad(player.IdentityId);

               

                //Check spawn count limit
                if (use != null && configs.MaxSpawnsForPlayer != 0 && use.Count >= configs.MaxSpawnsForPlayer)
                {
                    message = $"{player.DisplayName} has reached their spawn limit!";
                    passedChecks = false;
                    break;
                }

                //Check each timer for individual players
                if (use != null && configs.SpawnTimer != 0 && use.LastUse + TimeSpan.FromMinutes(configs.SpawnTimer) > DateTime.Now)
                {

                    TimeSpan TimeLeft = DateTime.Now - (use.LastUse + TimeSpan.FromMinutes(configs.SpawnTimer));
                    message = $"{player.DisplayName} has {TimeLeft.ToString(@"hh\:mm\:ss")} left until next spawn use!";
                    passedChecks = false;
                    break;
                }


                //Check each players promote level

                /*

                if (Player.PromoteLevel <= Configs.MinimumRole)
                {
                    Message = $"{Player.DisplayName} doesnt have the required role!";
                    PassedChecks = false;
                    break;
                }
                */
            }

            //If they didnt pass the checks, broadcast why and return
            if (!passedChecks)
            {
                BroadcastMessage(message);
                return;
            }

            // If all players passed the checks, check the current timer status
            if (currentTimer <= 0)
            {
                //If timer is less than or equal to 0, send all clients to server via spawn message
                BeginSpawn();
            }
            else
            {
                //Display the current timer countdown

                BroadcastMessage($"Spawning at {terminalBlock.DisplayNameText} in {currentTimer}");
            }
        }

        


        private void BeginSpawn()
        {
            List<ulong> allSteamIDs = new List<ulong>();
            foreach (IMyPlayer player in containedPlayers)
                allSteamIDs.Add(player.SteamUserId);

            ServerSpawnMessage message = new ServerSpawnMessage();
            message.ToServerID = configs.TargetServerID;
            message.ShipPrefabName = configs.PrefabName;
            message.ScriptName = configs.ScriptName;
            message.ContainedPlayers = allSteamIDs;
            message.CustomData = configs.CustomData;

            configs.AddPlayerUses(containedPlayers);

            message.SendMessageToServer();

            currentTimer = 5;

            SaveData();
            //CloseAllPlayers();
        }



        private void BroadcastMessage(string message)
        {
            foreach (IMyPlayer player in containedPlayers)
            {
                if (player == null)
                    continue;

                MyVisualScriptLogicProvider.ShowNotification(message, DissapearTime, Font, player.IdentityId);
            }
        }

        private void AddCustomDataConfigs()
        {

            string customData = terminalBlock.CustomData;
            int customDataLines = customData.Split(new[] { "\r\n", "\r", "\n", Environment.NewLine }, StringSplitOptions.None).Length;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("PrefabName:");
            builder.AppendLine("ScriptName:");
            builder.AppendLine("ToServerID:1");
            builder.AppendLine("MinPlayers:0");
            builder.AppendLine("MaxPlayers:0");
            builder.AppendLine("MaxSpawnsForPlayer:0");
            builder.AppendLine("SpawnTimer (min):0");
            builder.AppendLine("MinRole:None");
            builder.AppendLine("CustomData:");

            string targetCustomData = builder.ToString();
            int targetLineCount = targetCustomData.Split(new[] { "\r\n", "\r", "\n", Environment.NewLine }, StringSplitOptions.None).Length;

            if (customDataLines < targetLineCount)
            {
                //MyAPIGateway.Utilities?.ShowMessage("SyncMod", "Updating new customdata!");
                terminalBlock.CustomData = targetCustomData;
            }

            GetCustomDataSettings();
        }

        private bool GetCustomDataSettings()
        {
            try
            {
                MatchCollection customDataMatches = RegCustomData.Matches(terminalBlock.CustomData);

                //Make sure we have the collections we need
                if (customDataMatches.Count < 9)
                {
                    if (string.IsNullOrEmpty(terminalBlock.CustomData))
                        AddCustomDataConfigs();

                    //AddCustomDataConfigs();
                    return false;
                }


                configs.PrefabName = customDataMatches[0].Groups[1].Value;
                configs.ScriptName = customDataMatches[1].Groups[1].Value;
                configs.TargetServerID = TryParseInt(customDataMatches[2].Groups[1].Value);
                configs.MinPlayers = TryParseInt(customDataMatches[3].Groups[1].Value);
                configs.MaxPlayers = TryParseInt(customDataMatches[4].Groups[1].Value);

                configs.MaxSpawnsForPlayer = TryParseInt(customDataMatches[5].Groups[1].Value);
                configs.SpawnTimer = TryParseDouble(customDataMatches[6].Groups[1].Value);

                try
                {
                    configs.MinimumRole = (MyPromoteLevel)Enum.Parse(typeof(MyPromoteLevel), customDataMatches[7].Groups[1].Value);
                
                }
                catch (Exception)
                {
                    configs.MinimumRole = MyPromoteLevel.None;
                    MyLog.Default?.WriteLineAndConsole($"NexusSyncMod: MyPromoteLevel: '{customDataMatches[7].Groups[1].Value}' was not in correct format!");
                }




                configs.CustomData = customDataMatches[8].Groups[1].Value;
                return true;

            }
            catch (Exception Ex)
            {
                MyLog.Default?.WriteLineAndConsole($"NexusSyncMod: {Ex.ToString()}");
            }

            return false;
        }

        private int TryParseInt(string input)
        {
            int result;
            if(!int.TryParse(input, out result))
            {
                result = 0;
                MyLog.Default?.WriteLineAndConsole($"NexusSyncMod: String '{input}' was not in correct format for int!");
            }
            return result;
        }

        private double TryParseDouble(string input)
        {
            double result;
            if (!double.TryParse(input, out result))
            {
                result = 0;
                MyLog.Default?.WriteLineAndConsole($"NexusSyncMod: String '{input}' was not in correct format for double!");
            }
            return result;

        }

        private bool LoadData()
        {
            string data;
            if (entity.Storage != null && entity.Storage.TryGetValue(StorageGUID, out data))
            {
                byte[] savedData = Convert.FromBase64String(data);

                try
                {
                    SpawnPadConfigs oldConfigs = MyAPIGateway.Utilities.SerializeFromBinary<SpawnPadConfigs>(savedData);
                    if (oldConfigs == null)
                        return true;

                    configs = oldConfigs;
                    //MyAPIGateway.Utilities?.ShowMessage("SyncMod", "Loading old configs!");
                    return true;

                }
                catch (Exception Ex)
                {
                    MyLog.Default?.WriteLineAndConsole($"SyncMod: {Ex.ToString()}");
                    return false;
                }

            }

            return false;
        }

        public void SaveData()
        {
            if (entity.Storage == null)
                entity.Storage = new MyModStorageComponent();

            byte[] newByteData = MyAPIGateway.Utilities.SerializeToBinary(configs);
            string base64string = Convert.ToBase64String(newByteData);
            entity.Storage[StorageGUID] = base64string;

            //MyAPIGateway.Utilities?.ShowMessage("SyncMod", "Saving data!");
        }

        private void CloseAllPlayers()
        {
            foreach (IMyPlayer player in containedPlayers)
                player.Character.Close();
        }

    }
}
