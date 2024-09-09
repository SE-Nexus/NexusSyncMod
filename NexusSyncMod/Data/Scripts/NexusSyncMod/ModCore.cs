using NexusSyncMod.Gates;
using NexusSyncMod.Respawn;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using NexusSyncMod.Render;
using Draygo.API;
using VRageMath;

namespace NexusSyncMod
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class ModCore : MySessionComponentBase
    {
        public const bool DEBUG = false; 

        private bool IsServer => DEBUG ? MyAPIGateway.Utilities.IsDedicated : MyAPIGateway.Session.IsServer;
        private RespawnScreen playerScreen = new RespawnScreen();
        private BorderRenderManager renderer = new BorderRenderManager();
        private GateVisuals gateVisuals = new GateVisuals();
        private HudAPIv2 hudApi;

        protected override void UnloadData()
        {
            if (IsServer)
                return;

            if (hudApi != null)
                hudApi.Unload();

            playerScreen.UnloadData();
            renderer.Unload();
            gateVisuals.UnloadData();
        }

        public override void LoadData()
        {
            if (IsServer)
                return;

            Log.Info("Server Running NexusV3...");
            playerScreen.Init();
            renderer.InitNetwork();
            gateVisuals.Init();
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
        }


        public override void UpdateBeforeSimulation()
        {
            if (IsServer)
                return;

            //TryShow("Added Cube block!");
            base.UpdateBeforeSimulation();
        }

        public override void UpdateAfterSimulation()
        {
            if (IsServer)
                return;

            //TryShow("Added Cube block!");
            if (MyAPIGateway.Session == null)
                return;



        }
    }

}
