using Sandbox.ModAPI;
using VRage.Utils;

namespace NexusSyncMod
{
    internal static class Log
    {
#if DEBUG
        private const bool EnableDebug = true;
#else
        private const bool EnableDebug = false;
#endif

        public static void Debug(string msg)
        {
            if(EnableDebug)
            {
                MyAPIGateway.Utilities?.ShowMessage("NexusMOD", msg);
                MyLog.Default?.WriteLineAndConsole("NexusMOD: " + msg);
            }
        }

        public static void Info(string msg)
        {
            if (EnableDebug)
                MyAPIGateway.Utilities?.ShowMessage("NexusMOD", msg);
            MyLog.Default?.WriteLineAndConsole("NexusMOD: " + msg);
        }

        public static void Warn(string msg)
        {
            if (EnableDebug)
                MyAPIGateway.Utilities?.ShowMessage("NexusMOD", msg);
            MyLog.Default?.WriteLineAndConsole("NexusMOD WARNING: " + msg);
        }

        public static void Error(string msg)
        {
            if (EnableDebug)
                MyAPIGateway.Utilities?.ShowMessage("NexusMOD", msg);
            MyLog.Default?.WriteLineAndConsole("NexusMOD ERROR: " + msg);
        }


    }
}
