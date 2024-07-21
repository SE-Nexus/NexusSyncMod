using ProtoBuf;
using System;
using System.Collections.Generic;
using VRage.Game.ModAPI;

namespace NexusSyncMod.SpawnPads
{
    [ProtoContract]
    public class SpawnPadConfigs
    {

        public string PrefabName;
        public string ScriptName;
        public int TargetServerID { get; set; } = 0;
        public int MinPlayers { get; set; } = 0;
        public int MaxPlayers { get; set; } = 0;

        public int MaxSpawnsForPlayer { get; set; } = 0;
        public double SpawnTimer { get; set; } = 0;

        public MyPromoteLevel MinimumRole;
        public string CustomData;

        [ProtoMember(1)]
        Dictionary<long, PlayerPadUse> PlayerUses = new Dictionary<long, PlayerPadUse>();

        public PlayerPadUse GetPlayerFromPad(long Player)
        {
            if (PlayerUses.ContainsKey(Player))
            {
                return PlayerUses[Player];
            }
            else
            {
                return null;
            }
        }


        public void AddPlayerUses(List<IMyPlayer> Players)
        {

            foreach (var Player in Players)
            {

                if (PlayerUses.ContainsKey(Player.IdentityId))
                {
                    PlayerUses[Player.IdentityId].Count++;
                    PlayerUses[Player.IdentityId].LastUse = DateTime.Now;
                }
                else
                {

                    PlayerPadUse Use = new PlayerPadUse();
                    PlayerUses.Add(Player.IdentityId, Use);
                }
            }
        }



        public SpawnPadConfigs() { }
    }
}
