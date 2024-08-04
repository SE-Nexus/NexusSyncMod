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

namespace NexusSyncMod
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class ModCore : MySessionComponentBase
    {
        private bool IsServer => MyAPIGateway.Multiplayer.IsServer;
        private RespawnScreen PlayerScreen;

        protected override void UnloadData()
        {
            if (IsServer)
                return;

            if (PlayerScreen != null)
                PlayerScreen.UnloadData();


            GateVisuals.UnloadData();
        }



        public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
        {
            if (IsServer)
                return;





            GateVisuals.Init();
            base.Init(sessionComponent);
        }


        public override void LoadData()
        {
            if (IsServer)
                return;


            Log.Info("Starting Systems! Madeby: Casimir");
            PlayerScreen = new RespawnScreen();
        }

        public override void Draw()
        {
            GateVisuals.Draw();
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
