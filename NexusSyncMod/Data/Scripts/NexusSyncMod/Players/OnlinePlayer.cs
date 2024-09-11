using ProtoBuf;

namespace NexusSyncMod.Players
{
    [ProtoContract]
    public class OnlinePlayer
    {
        [ProtoMember(1)] public string PlayerName;

        [ProtoMember(2)] public ulong SteamID;

        [ProtoMember(3)] public long IdentityID;

        [ProtoMember(4)] public int OnServer;


        public string ServerName;

        public OnlinePlayer(string PlayerName, ulong SteamID, long IdentityID, int OnServer)
        {
            this.PlayerName = PlayerName;
            this.SteamID = SteamID;
            this.IdentityID = IdentityID;
            this.OnServer = OnServer;
        }

        public OnlinePlayer()
        {
        }
    }

}
