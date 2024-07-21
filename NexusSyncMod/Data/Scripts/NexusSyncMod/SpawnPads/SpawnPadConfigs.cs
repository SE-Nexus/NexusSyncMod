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

        public PlayerPadUse GetPlayerFromPad(long player)
        {
            if (PlayerUses.ContainsKey(player))
            {
                return PlayerUses[player];
            }
            else
            {
                return null;
            }
        }


        public void AddPlayerUses(List<IMyPlayer> players)
        {

            foreach (IMyPlayer player in players)
            {
                PlayerPadUse use;
                if(PlayerUses.TryGetValue(player.IdentityId, out use))
                {
                    use.Count++;
                    use.LastUse = DateTime.Now;
                }
                else
                {
                    use = new PlayerPadUse();
                    PlayerUses.Add(player.IdentityId, use);
                }
            }
        }



        public SpawnPadConfigs() { }
    }
}
