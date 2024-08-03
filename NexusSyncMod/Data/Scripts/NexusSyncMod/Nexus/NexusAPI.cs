using System.Collections.Generic;
using VRage.Game;
using VRageMath;

namespace NexusSyncMod.Nexus
{
    public class NexusAPI
    {
        public ushort CrossServerModID;

        /*  For recieving custom messages you have to register a message handler with a different unique ID then what you use server to client. (It should be the same as this class)
         *  
         *  NexusAPI(5432){
         *  CrossServerModID = 5432
         *  }
         *  
         *  
         *  Register this somewhere in your comms code. (This will only be raised when it recieves a message from another server)
         *  MyAPIGateway.Multiplayer.RegisterMessageHandler(5432, MessageHandler);
         */




        public NexusAPI(ushort SocketID)
        {
            CrossServerModID = SocketID;
        }

        public static bool IsRunningNexus()
        {
            return false;
        }

        public static bool IsPlayerOnline(long IdentityID)
        {
            return false;
        }

        private static List<object[]> GetSectorsObject()
        {
            List<object[]> APISectors = new List<object[]>();
            return APISectors;
        }

        private static List<object[]> GetAllOnlinePlayersObject()
        {
            List<object[]> OnlinePlayers = new List<object[]>();
            return OnlinePlayers;
        }

        private static List<object[]> GetAllServersObject()
        {
            List<object[]> Servers = new List<object[]>();
            return Servers;

        }
        private static List<object[]> GetAllOnlineServersObject()
        {
            List<object[]> Servers = new List<object[]>();
            return Servers;

        }

        private static object[] GetThisServerObject()
        {
            object[] OnlinePlayers = new object[4];
            return OnlinePlayers;
        }


        public static Server GetThisServer()
        {
            object[] obj = GetThisServerObject();
            return new Server((string)obj[0], (int)obj[1], (int)obj[2], (string)obj[3]);
        }

        public static List<Sector> GetSectors()
        {
            List<object[]> Objs = GetSectorsObject();

            List<Sector> Sectors = new List<Sector>();
            foreach (object[] obj in Objs)
            {
                Sectors.Add(new Sector((string)obj[0], (string)obj[1], (int)obj[2], (bool)obj[3], (Vector3D)obj[4], (double)obj[5], (int)obj[6]));
            }
            return Sectors;
        }


        public static int GetServerIDFromPosition(Vector3D Position)
        {
            return 0;
        }


        public static List<Player> GetAllOnlinePlayers()
        {
            List<object[]> Objs = GetAllOnlinePlayersObject();

            List<Player> Players = new List<Player>();
            foreach (object[] obj in Objs)
            {
                Players.Add(new Player((string)obj[0], (ulong)obj[1], (long)obj[2], (int)obj[3]));
            }
            return Players;
        }


        public static List<Server> GetAllServers()
        {
            List<object[]> Objs = GetAllServersObject();

            List<Server> Servers = new List<Server>();
            foreach (object[] obj in Objs)
            {
                Servers.Add(new Server((string)obj[0], (int)obj[1], (int)obj[2], (string)obj[3]));
            }
            return Servers;
        }
        public static List<Server> GetAllOnlineServers()
        {
            List<object[]> Objs = GetAllOnlineServersObject();

            List<Server> Servers = new List<Server>();
            foreach (object[] obj in Objs)
            {
                Servers.Add(new Server((string)obj[0], (int)obj[1], (int)obj[2], (float)obj[3], (int)obj[4], (List<ulong>)obj[5]));
            }
            return Servers;
        }



        public static bool IsServerOnline(int ServerID)
        {
            return false;
        }
        public static void BackupGrid(List<MyObjectBuilder_CubeGrid> GridObjectBuilders, long OnwerIdentity)
        {
            return;
        }
        public static void SendChatMessageToDiscord(ulong ChannelID, string Author, string Message) { }
        public static void SendEmbedMessageToDiscord(ulong ChannelID, string EmbedTitle, string EmbedMsg, string EmbedFooter, string EmbedColor = null) { }

        public void SendMessageToServer(int ServerID, byte[] Message)
        {
            return;
        }

        public void SendMessageToAllServers(byte[] Message)
        {
            return;
        }





    }
}
