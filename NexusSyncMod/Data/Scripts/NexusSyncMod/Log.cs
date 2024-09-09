using Sandbox.ModAPI;
using VRage.Utils;

namespace NexusSyncMod
{
    internal static class Log
    {
        public static void Debug(string msg)
        {
            if(ModCore.DEBUG)
            {
                MyAPIGateway.Utilities?.ShowMessage("NexusMOD", msg);
                MyLog.Default?.WriteLineAndConsole("NexusMOD: " + msg);
            }
        }

        public static void Info(string msg)
        {
            if (ModCore.DEBUG)
                MyAPIGateway.Utilities?.ShowMessage("NexusMOD", msg);
            MyLog.Default?.WriteLineAndConsole("NexusMOD: " + msg);
        }

        public static void Warn(string msg)
        {
            if (ModCore.DEBUG)
                MyAPIGateway.Utilities?.ShowMessage("NexusMOD", msg);
            MyLog.Default?.WriteLineAndConsole("NexusMOD WARNING: " + msg);
        }

        public static void Error(string msg)
        {
            if (ModCore.DEBUG)
                MyAPIGateway.Utilities?.ShowMessage("NexusMOD", msg);
            MyLog.Default?.WriteLineAndConsole("NexusMOD ERROR: " + msg);
        }


    }
}
