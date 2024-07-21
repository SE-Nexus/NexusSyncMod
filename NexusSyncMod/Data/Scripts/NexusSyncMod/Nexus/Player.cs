namespace NexusSyncMod.Nexus
{
    public class Player
    {

        public readonly string PlayerName;

        public readonly ulong SteamID;

        public readonly long IdentityID;

        public readonly int OnServer;

        public Player(string PlayerName, ulong SteamID, long IdentityID, int OnServer)
        {
            this.PlayerName = PlayerName;
            this.SteamID = SteamID;
            this.IdentityID = IdentityID;
            this.OnServer = OnServer;
        }
    }
}
