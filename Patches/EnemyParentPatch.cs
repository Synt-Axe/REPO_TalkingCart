using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TalkingCart.Patches.RoundDirectorPatch;

namespace TalkingCart.Patches
{
    [HarmonyPatch(typeof(EnemyParent))]
    class EnemyParentPatch
    {
        [HarmonyPatch("Despawn")]
        [HarmonyPostfix]
        static void EnemyDespawnPatch(EnemyParent __instance)
        {
            if (RoundDirectorPatch.initialEnemiesCommunicated)
            {
                if (__instance.enemyName == "Banger" || __instance.enemyName == "Gnome") return; // Gnomes and Bangers

                int enemyInd = RoundDirectorPatch.enemyParentList.IndexOf(__instance);
                RoundDirectorPatch.currentEnemyStatus[enemyInd] = EnemyStatus.Absent;

                TalkingCartBase.mls.LogInfo($"Enemy Despawn Called: {__instance.enemyName}");
            }
        }

        [HarmonyPatch("Spawn")]
        [HarmonyPostfix]
        static void EnemyRespawnPatch(EnemyParent __instance)
        {
            if (RoundDirectorPatch.initialEnemiesCommunicated)
            {
                if (__instance.enemyName == "Banger" || __instance.enemyName == "Gnome") return; // Gnomes and Bangers

                int enemyInd = RoundDirectorPatch.enemyParentList.IndexOf(__instance);
                RoundDirectorPatch.currentEnemyStatus[enemyInd] = EnemyStatus.Present;

                TalkingCartBase.mls.LogInfo($"Enemy Respawn Called: {__instance.enemyName}");
            }
        }
    }
}
