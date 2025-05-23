using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkingCart.Patches
{
    [HarmonyPatch(typeof(ValuableObject))]
    class ValuableObjectsRecords
    {
        public static List<ValuableObject> levelValuables = new List<ValuableObject>();

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void ValuableStartPatch(ValuableObject __instance)
        {
            levelValuables.Add(__instance);
        }
    }
}
