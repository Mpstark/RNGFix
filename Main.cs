using System.Reflection;
using Harmony;
using HBS.Logging;

namespace RNGFix
{
    public static class Main
    {
        internal static ILog HBSLog;

        public static void Init(string modDir, string settings)
        {
            var harmony = HarmonyInstance.Create("io.github.mpstark.RNGFix");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            HBSLog = Logger.GetLogger("RNGFix");
        }
    }
}
