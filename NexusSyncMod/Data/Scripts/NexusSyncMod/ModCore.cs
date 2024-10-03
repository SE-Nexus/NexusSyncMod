using NexusSyncMod.Gates;
using NexusSyncMod.Respawn;
using Sandbox.ModAPI;
using VRage.Game.Components;
using NexusSyncMod.Render;
using Draygo.API;
using NexusSyncMod.Players;

namespace NexusSyncMod
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class ModCore : MySessionComponentBase
    {
        public const bool DEBUG = false; 

        private bool IsServer => DEBUG ? MyAPIGateway.Utilities.IsDedicated : MyAPIGateway.Session.IsServer;
        private RespawnScreen respawnScreen = new RespawnScreen();
        private BorderRenderManager renderer = new BorderRenderManager();
        private GateVisuals gateVisuals = new GateVisuals();
        private PlayersHud playersList = new PlayersHud();
        private HudAPIv2 hudApi;

        protected override void UnloadData()
        {
            if (IsServer)
                return;

            if (hudApi != null)
                hudApi.Unload();

            respawnScreen.UnloadData();
            renderer.Unload();
            gateVisuals.UnloadData();
            playersList.Unload();
        }

        public override void LoadData()
        {
            if (IsServer)
                return;

            Log.Info("Server Running NexusV3...");
            if(DEBUG)
                Log.Info("Debug mode enabled");
            respawnScreen.Init();
            renderer.InitNetwork();
            gateVisuals.Init();
            playersList.Init();
        }

        public override void Draw()
        {
            gateVisuals.Draw();
            renderer.Draw();
        }

        public override void BeforeStart()
        {
            hudApi = new HudAPIv2(OnHudReady);
        }

        public void OnHudReady()
        {
            renderer.OnHudReady();
            playersList.InitHud();
        }


        public override void UpdateAfterSimulation()
        {
            if (IsServer)
                return;

>>>>>>> 7854161e3ebf13f5ca53e453bc602c31223c6546
        }
    }

}
