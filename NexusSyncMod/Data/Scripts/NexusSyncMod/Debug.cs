using Sandbox.ModAPI;
using VRage.Utils;

namespace NexusSyncMod
{
    public class Debug
    {
        private static bool EnableDebug = true;
        public static void Write(string msg)
        {
            if (EnableDebug)
            {
                MyAPIGateway.Utilities.ShowMessage("NexusMOD", msg);
                MyLog.Default.WriteLineAndConsole("NexusMOD: " + msg);


            }
        }
    }
}
