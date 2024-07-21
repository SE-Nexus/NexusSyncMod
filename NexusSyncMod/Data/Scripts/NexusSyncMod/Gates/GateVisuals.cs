using Sandbox.ModAPI;
using System.Collections.Generic;
using VRage.Game;
using VRage.Utils;
using VRageMath;

namespace NexusSyncMod.Gates
{
    public static class GateVisuals
    {
        // The visual effects for the gates were designed by Klime for Nexus who did a fantastic job.


        private static List<RotatingParticle> all_effects = new List<RotatingParticle>();
        private static float preset_radius = 800; //radius in meters
        const ushort GateNetID = 2937;
        private static  int timer = 0;

        public class RotatingParticle
        {
            public MyParticleEffect effect;
            public float current_angle;
            public MatrixD initial_matrix; //Initial position
            public MatrixD center_matrix; //Center position to orbit around
            public float radius;
            public RotatingParticle(MyParticleEffect effect, float current_angle, MatrixD initial_matrix, MatrixD center_matrix, float radius)
            {
                this.effect = effect;
                this.current_angle = current_angle;
                this.initial_matrix = initial_matrix;
                this.center_matrix = center_matrix;
                this.radius = radius;
            }
        }



        public static void Init()
        {
            if (!MyAPIGateway.Session.IsServer)
            {
                MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(GateNetID, MessageHandler);

                MyLog.Default?.WriteLineAndConsole($"GateSystem: Initilized");
                // MyAPIGateway.Utilities.MessageEntered += Utilities_MessageEntered; // Chat messages in offline mode only (for debug)
            }
        }



        private static void MessageHandler(ushort arg1, byte[] arg2, ulong arg3, bool arg4)
        {
            //Server sends all gate particle effects
            GateVisualData RecievedMessage = MyAPIGateway.Utilities.SerializeFromBinary<GateVisualData>(arg2);

            MyLog.Default?.WriteLineAndConsole($"GateSystem: Recieved Message from server!");

            if (RecievedMessage == null)
                return;


            //Stop and clear all previous effects
            RemoveAllEffect();


                
            foreach(var GateData in RecievedMessage.AllGates)
            {

                string EffectName = GateData.ParticleEffect;
                MatrixD Center_Matrix;

                Vector3D  forward = Vector3D.Cross(Vector3D.CalculatePerpendicularVector(-GateData.Direction), GateData.Direction);
                //MatrixD.creeat

                

                Center_Matrix = MatrixD.CreateWorld(GateData.Center, Vector3D.CalculatePerpendicularVector(GateData.Direction), -GateData.Direction); // MatrixD.CreateTranslation(GateData.Center);

             
                //End effect should be smaller
                if (EffectName.Contains("end"))
                {
                    GateData.Size = 160;
                    Center_Matrix = MatrixD.CreateWorld(GateData.Center, Vector3D.CalculatePerpendicularVector(GateData.Direction), GateData.Direction); // MatrixD.CreateTranslation(GateData.Center);
                }

               

             
                MatrixD initial_matrix = Center_Matrix;

                initial_matrix.Translation += initial_matrix.Right * preset_radius; //Offset to the right
                Vector3D wm_pos = initial_matrix.Translation;

                MyParticleEffect effect;



                MyParticlesManager.TryCreateParticleEffect(EffectName, ref initial_matrix, ref wm_pos, uint.MaxValue, out effect);
                if (effect != null)
                {
                    RotatingParticle rotating_particle = new RotatingParticle(effect, 0, initial_matrix, Center_Matrix, GateData.Size); // Create particle and add to list
                    all_effects.Add(rotating_particle);
                    //MyAPIGateway.Utilities.ShowNotification("Created Effect", 2000, "White");
                }
            }
        }



        private static void RemoveAllEffect()
        {
            foreach (var rotating_particle in all_effects)
            {
                rotating_particle.effect.Stop(true);
                rotating_particle.effect.StopEmitting();
                rotating_particle.effect.StopLights();
            }
            MyAPIGateway.Utilities.ShowNotification("Removed all effects", 2000, "White");
            all_effects.Clear();
        }



        public static void Draw()
        {
            if (MyAPIGateway.Utilities.IsDedicated) //Don't want particles on the DS
            {
                return;
            }

            foreach (var rotating_particle in all_effects)
            {
                Vector3D angle = Vector3D.Rotate(rotating_particle.initial_matrix.Translation - rotating_particle.center_matrix.Translation,
                    MatrixD.CreateFromAxisAngle(rotating_particle.center_matrix.Up, rotating_particle.current_angle)); //Rotate the particle emitter every tick

                Vector3D final_pos = rotating_particle.center_matrix.Translation + Vector3D.Normalize(angle) * rotating_particle.radius;
                Vector3D final_up = Vector3D.Normalize(rotating_particle.center_matrix.Translation - final_pos);




                rotating_particle.effect.WorldMatrix = MatrixD.CreateWorld(final_pos, final_up, rotating_particle.effect.WorldMatrix.Up); //Final matrix
                rotating_particle.current_angle += 5f; //Increase the angle every tick

                //MyVisualScriptLogicProvider.CreateLightning(final_pos);


            }
            timer += 1;
        }

        public static void UnloadData()
        {
            RemoveAllEffect();

            MyAPIGateway.Multiplayer.UnregisterSecureMessageHandler(GateNetID, MessageHandler);
        }
    }
}
