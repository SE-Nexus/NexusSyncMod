using Draygo.API;
using NexusSyncMod.Data.Scripts.NexusSyncMod.Players;
using NexusSyncMod.Gates;
using NexusSyncMod.Players;
using NexusSyncMod.Render;
using NexusSyncMod.Respawn;
using Sandbox.ModAPI;
using VRage.Game.Components;

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
        private PlayerStatusHud playerStatusHud = new PlayerStatusHud();
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
            playerStatusHud.Unload();
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
            playerStatusHud.Init();
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
            playerStatusHud.InitHud();
        }


        public override void UpdateAfterSimulation()
        {
            if (IsServer)
                return;

        }
    }

}
