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

    //[MyEntityComponentDescriptor(typeof(MyObjectBuilder_RadioAntenna), true, new string[] { "SpawnPadSingle", "SpawnPadMulti" })]
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class ModCore : MySessionComponentBase
    {
        private bool IsServer => MyAPIGateway.Multiplayer.IsServer;
        private const int MaxTimer = 60;

        private int counter = 0;

        private List<string> AllSpawnTypes = new List<string>() { "SpawnPadMulti" };


        private RespawnScreen PlayerScreen;

        private void Init()
        {
            //TryShow("Attached Entity Events");
        }

        protected override void UnloadData()
        {
            if (PlayerScreen != null)
                PlayerScreen.UnloadData();

            if (!IsServer)
                GateVisuals.UnloadData();
        }
        


        public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
        {
            GateVisuals.Init();
            base.Init(sessionComponent);
        }


        public override void LoadData()
        {
            if (!IsServer)
            {
                Log.Info("Starting Systems! Madeby: Casimir");
                PlayerScreen = new RespawnScreen();
                return;
            }
                

            //Init();
        }

        public override void Draw()
        {
            GateVisuals.Draw();
        }



        public override void UpdateBeforeSimulation()
        {
            //TryShow("Added Cube block!");
            base.UpdateBeforeSimulation();
        }

        public override void UpdateAfterSimulation()
        {
            //TryShow("Added Cube block!");
            if (MyAPIGateway.Session == null)
                return;


            if (counter <= MaxTimer)
            {
                //TryShow($"Timer {Counnter}");
                counter++;
                return;
            }




            if (IsServer)
            {
                Update1000Server();
            }


            counter = 0;
        }




        private void Update1000Server()
        {

        }

        private void Update1000Client()
        {
            //TryShow("Updating Pad Client");
        }


    }

}
