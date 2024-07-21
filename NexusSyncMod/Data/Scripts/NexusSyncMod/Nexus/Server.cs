using System.Collections.Generic;

namespace NexusSyncMod.Nexus
{
    public class Server
    {
        public readonly string Name;
        public readonly int ServerID;
        public readonly int ServerType;
        public readonly string ServerIP;

        public readonly int MaxPlayers;
        public readonly float ServerSS;
        public readonly int TotalGrids;
        public readonly List<ulong> ReservedPlayers;

        /*  Possible Server Types
         * 
         *  0 - SyncedSectored
         *  1 - SyncedNon-Sectored
         *  2 - Non-Synced & Non-Sectored
         * 
         */


        public Server(string Name, int ServerID, int ServerType, string IP)
        {
            this.Name = Name;
            this.ServerID = ServerID;
            this.ServerType = ServerType;
            this.ServerIP = IP;
        }


        //Online Server
        public Server(string Name, int ServerID, int MaxPlayers, float SimSpeed, int TotalGrids, List<ulong> ReservedPlayers)
        {
            this.Name = Name;
            this.ServerID = ServerID;
            this.MaxPlayers = MaxPlayers;
            this.ServerSS = SimSpeed;
            this.TotalGrids = TotalGrids;
            this.ReservedPlayers = ReservedPlayers;
        }

    }
}
