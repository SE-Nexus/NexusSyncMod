using Sandbox.ModAPI;
using System.Collections.Generic;
using VRage.Game;
using VRage.Utils;
using VRageMath;

namespace NexusSyncMod.Gates
{
    public class GateVisuals
    {
        // The visual effects for the gates were designed by Klime for Nexus who did a fantastic job.


        private readonly List<RotatingParticle> all_effects = new List<RotatingParticle>();

        private const float preset_radius = 800; //radius in meters
        private const ushort GateNetID = 2937;

        public void Init()
        {
            if (!MyAPIGateway.Session.IsServer)
            {
                MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(GateNetID, MessageHandler);
                Log.Info($"GateSystem: Initilized");
            }
        }



        private void MessageHandler(ushort packetId, byte[] data, ulong senderId, bool fromServer)
        {
            //Server sends all gate particle effects
            GateVisualMsg recievedMessage = MyAPIGateway.Utilities.SerializeFromBinary<GateVisualMsg>(data);

            Log.Debug($"GateSystem: Received Message from server!");

            if (recievedMessage == null)
                return;


            //Stop and clear all previous effects
            RemoveAllEffect();


                
            foreach(GateVisualData gateData in recievedMessage.AllGates)
            {

                string effectName = gateData.ParticleEffect;
                MatrixD centerMatrix;

                Vector3D forward = Vector3D.Cross(Vector3D.CalculatePerpendicularVector(-gateData.Direction), gateData.Direction);

                

                centerMatrix = MatrixD.CreateWorld(gateData.Center, Vector3D.CalculatePerpendicularVector(gateData.Direction), -gateData.Direction); // MatrixD.CreateTranslation(GateData.Center);

             
                //End effect should be smaller
                if (effectName.Contains("end"))
                {
                    
                    centerMatrix = MatrixD.CreateWorld(gateData.Center, Vector3D.CalculatePerpendicularVector(gateData.Direction), gateData.Direction); // MatrixD.CreateTranslation(GateData.Center);
                }

               

             
                MatrixD initialMatrix = centerMatrix;

                initialMatrix.Translation += initialMatrix.Right * preset_radius; //Offset to the right
                Vector3D wm_pos = initialMatrix.Translation;

                MyParticleEffect effect;
                MyParticlesManager.TryCreateParticleEffect(effectName, ref initialMatrix, ref wm_pos, uint.MaxValue, out effect);
                if (effect != null)
                {
                    effect.UserScale = gateData.Size / 500;
                    effect.UserVelocityMultiplier = gateData.Size / 500;
                    effect.UserRadiusMultiplier = VRageMath.MathHelper.Clamp(500 / gateData.Size, 1, 4);
                    //effect.UserLifeMultiplier = Math.Min(Math.Max(gateData.Size / 500, 0.4f), 1);
                    //effect.UserVelocityMultiplier = gateData.Size / 800;
                    //effect.UserLifeMultiplier = gateData.Size / 800;
                    all_effects.Add(new RotatingParticle(effect, 0, initialMatrix, centerMatrix, gateData.Size));
                }
            }
        }



        private void RemoveAllEffect()
        {
            foreach (RotatingParticle rotating_particle in all_effects)
            {
                rotating_particle.effect.Stop(true);
                rotating_particle.effect.StopEmitting();
                rotating_particle.effect.StopLights();
            }
            //MyAPIGateway.Utilities.ShowNotification("Removed all effects", 2000, "White");
            all_effects.Clear();
        }



        public void Draw()
        {
            if (MyAPIGateway.Utilities.IsDedicated) //Don't want particles on the DS
                return;

            foreach (RotatingParticle rotating_particle in all_effects)
            {
                Vector3D angle = Vector3D.Rotate(rotating_particle.initial_matrix.Translation - rotating_particle.center_matrix.Translation,
                    MatrixD.CreateFromAxisAngle(rotating_particle.center_matrix.Up, rotating_particle.current_angle)); //Rotate the particle emitter every tick

                Vector3D final_pos = rotating_particle.center_matrix.Translation + Vector3D.Normalize(angle) * rotating_particle.radius;
                Vector3D final_up = Vector3D.Normalize(rotating_particle.center_matrix.Translation - final_pos);




                rotating_particle.effect.WorldMatrix = MatrixD.CreateWorld(final_pos, final_up, rotating_particle.effect.WorldMatrix.Up); //Final matrix
                rotating_particle.current_angle += 5f; //Increase the angle every tick

                //MyVisualScriptLogicProvider.CreateLightning(final_pos);


            }
        }

        public void UnloadData()
        {
            RemoveAllEffect();

            MyAPIGateway.Multiplayer.UnregisterSecureMessageHandler(GateNetID, MessageHandler);
        }



        private class RotatingParticle
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
    }
}
