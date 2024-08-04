using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;

namespace NexusSyncMod.Respawn
{
    public class RespawnScreen
    {
        /*  Handles Grids in the Respawn Screen from other servers
         * 
         * 
         * 
         */

        public const ushort NETWORK_ID = 2935;

        public List<IMyEntity> RenderedGrids = new List<IMyEntity>();


        private int MaxGrids = 0;
        private HashSet<IMyEntity> _spawned;


        public RespawnScreen()
        {
            MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(NETWORK_ID, MessageHandler);
        }

        private void MessageHandler(ushort packetId, byte[] data, ulong senderId, bool fromServer)
        {
            try
            {
                ServerMessage recievedMessage = MyAPIGateway.Utilities.SerializeFromBinary<ServerMessage>(data);
                IMyPlayer lient = MyAPIGateway.Session.LocalHumanPlayer;
                ulong steamId = lient.SteamUserId;

                if (recievedMessage.ClearRenderedGrids)
                {
                    foreach (IMyEntity grid in RenderedGrids)
                        grid?.Close();

                    RenderedGrids.Clear();
                    return;
                }


                if (recievedMessage.PlayerSteamID == steamId && recievedMessage != null)
                {
                    //Debug.Write("GridBuildersCountA: " + RecievedMessage.GridBuilders.Count);

                    if (recievedMessage.GridBuilders.Count == 0)
                        return;



                    foreach (ClientGridBuilder builder in recievedMessage.GridBuilders)
                    {
                        List<MyObjectBuilder_CubeGrid> totalGrids = builder.Grids;
                        //Debug.Write("TotalGrids: " + TotalGrids.Count);

                        if (totalGrids.Count == 0)
                            continue;

                        //MyEntities.RemapObjectBuilderCollection(TotalGrids);
                        MaxGrids = totalGrids.Count;
                        _spawned = new HashSet<IMyEntity>();
                        foreach (MyObjectBuilder_CubeGrid grid in totalGrids)
                        {
                            //MyAPIGateway.Entities.RemapObjectBuilder(grid);
                            if (MyEntities.EntityExists(grid.EntityId))
                                continue;


                            MyAPIGateway.Entities.CreateFromObjectBuilderParallel(grid, false, Increment);

                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error("Error during message received! \n" + ex);
            }
        }


        public void Increment(IMyEntity entity)
        {


            MyEntity Ent = (MyEntity)entity;
            Ent.IsPreview = false;
            Ent.SyncFlag = false;
            Ent.Save = false;


            //Debug.Write("A");

            _spawned.Add(Ent);
            //Debug.Write("B");
            if (_spawned.Count < MaxGrids)
                return;

            //Debug.Write("C");


            MyAPIGateway.Utilities.InvokeOnGameThread(() =>
            {
                try
                {
                    foreach (IMyEntity g in _spawned)
                    {
                        if (g == null)
                            continue;

                        RenderedGrids.Add(g);
                        MyAPIGateway.Entities.AddEntity(g, true);
                    }

                }
                catch (Exception)
                {
                    //If we get an error, it could be caused by:
                    // 1. Corrupt/invalid grid
                    // 2. Duplicate ID which could be caused by grid already being loaded, or least likely, grid with that ID already exsits client side
                    //Debug.Write(ex.ToString());
                }
            });


            MaxGrids = 0;
        }



        public void UnloadData()
        {
            try
            {
                MyAPIGateway.Multiplayer.UnregisterSecureMessageHandler(NETWORK_ID, MessageHandler);
            }
            catch (Exception)
            {
                //MyLog.Default.WriteLineAndConsole("Cannot remove event Handlers! Are they already removed?1" + a);
            }
        }


    }
}
