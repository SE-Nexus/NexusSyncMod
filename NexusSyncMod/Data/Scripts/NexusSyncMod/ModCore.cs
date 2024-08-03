using NexusSyncMod.Gates;
using NexusSyncMod.Respawn;
using NexusSyncMod.SpawnPads;
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


        private Dictionary<MyEntity, SpawnPad> allSpawnsInServer = new Dictionary<MyEntity, SpawnPad>();

        private void Init()
        {
            MyEntities.OnEntityCreate += MyEntities_OnEntityCreate;
            MyEntities.OnEntityRemove += MyEntities_OnEntityRemove;
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
                PlayerScreen = new RespawnScreen();
                return;
            }
                

            //Init();
        }

        public override void Draw()
        {
            GateVisuals.Draw();
        }



        private void MyEntities_OnEntityRemove(VRage.Game.Entity.MyEntity e)
        {
            if (!(e is IMyRadioAntenna))
                return;

            //We only want blocks that match our blocks
            IMyCubeBlock Block = (IMyCubeBlock)e;
            string SubType = Block.BlockDefinition.SubtypeId;
            if (!AllSpawnTypes.Contains(SubType))
                return;

            if (allSpawnsInServer.ContainsKey(e))
                allSpawnsInServer.Remove(e);


            //TryShow("Removed SpawnPad block!");

        }

        private void MyEntities_OnEntityCreate(VRage.Game.Entity.MyEntity e)
        {
            if (!(e is IMyRadioAntenna))
                return;

            //We only want blocks that match our blocks
            IMyCubeBlock Block = (IMyCubeBlock)e;
            string SubType = Block.BlockDefinition.SubtypeId;

            if (!AllSpawnTypes.Contains(SubType))
                return;


            allSpawnsInServer.Add(e, new SpawnPad(e));
            //TryShow("Added SpawnPad block!");
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
            foreach (SpawnPad pad in allSpawnsInServer.Values)
                pad.Update();
        }

        private void Update1000Client()
        {
            //TryShow("Updating Pad Client");
        }


    }

}
