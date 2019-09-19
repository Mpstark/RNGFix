using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BattleTech;
using BattleTech.Data;
using Harmony;
using UnityEngine;
using Random = System.Random;

namespace RNGFix
{
    public static class Main
    {
        private static readonly Random RNG = new Random(); // not threadsafe
        //internal static ILog HBSLog;

        public static void Init()
        {
            //HBSLog = Logger.GetLogger("RNGFix");
            var harmony = HarmonyInstance.Create("io.github.mpstark.RNGFix");

            var patch = typeof(Main).GetMethod("Patch");
            if (patch == null)
                return;

            var types = new[]
            {
                typeof(UpgradeDef_MDD), typeof(WeaponDef_MDD), typeof(UnitResult),
                typeof(DynamicLanceDifficulty_MDD), typeof(LanceDef_MDD), typeof(PilotDef_MDD),
                typeof(UnitDef_MDD), typeof(string), typeof(ChassisLocations), typeof(Pilot),
                typeof(MechDef), typeof(EncounterLayer_MDD), typeof(Faction), typeof(PilotDef),
                typeof(StarSystem), typeof(Transform)
            };

            var utilityMethods = typeof(Utilities)
                .GetMethods(BindingFlags.Public | BindingFlags.Static);
            var originalMethod = utilityMethods.First(m => m.Name == "Shuffle" && m.GetParameters().Length == 1);

            foreach (var type in types)
            {
                var patchGeneric = patch.MakeGenericMethod(type);
                var originalGeneric = originalMethod.MakeGenericMethod(type);

                //HBSLog.Log($"Patching Shuffle<{type.Name}>");
                harmony.Patch(originalGeneric, new HarmonyMethod(patchGeneric));
            }
        }

        // ReSharper disable once UnusedMember.Local
        public static bool Patch<T>(List<T> list)
        {
            // from https://stackoverflow.com/questions/273313/randomize-a-listt
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = RNG.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            //HBSLog.Log($"Shuffling: {list}");
            return false;
        }
    }
}
