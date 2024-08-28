using NexusSyncMod.Gates;
using NexusSyncMod.Respawn;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System.Collections.Generic;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.Utils;
using NexusSyncMod.Render;

namespace NexusSyncMod
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class ModCore : MySessionComponentBase
    {
        private bool IsServer => MyAPIGateway.Multiplayer.IsServer;
        private RespawnScreen playerScreen = new RespawnScreen();
        private BorderRenderManager renderer = new BorderRenderManager();
        private GateVisuals gateVisuals = new GateVisuals();

        protected override void UnloadData()
        {
            if (IsServer)
                return;

            playerScreen.UnloadData();
            renderer.Unload();
            gateVisuals.UnloadData();
        }



        public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
        {
            if (IsServer)
                return;

            renderer.InitNetwork();
            gateVisuals.Init();
            base.Init(sessionComponent);
        }


        public override void LoadData()
        {
            if (IsServer)
                return;

            Log.Info("Starting Systems! Madeby: Casimir");
            playerScreen = new RespawnScreen();
        }

        public override void Draw()
        {
            gateVisuals.Draw();
            renderer.Draw();
        }

        public override void BeforeStart()
        {
            renderer.InitHud();
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
