using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TalkingCart.Patches.RoundDirectorPatch;

namespace TalkingCart.Patches
{
    [HarmonyPatch(typeof(Enemy))]
    class EnemyPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void GetEnemyListedPatch(Enemy __instance, ref EnemyParent ___EnemyParent)
        {
            TalkingCartBase.mls.LogInfo($"Enemy Spawned: {___EnemyParent.enemyName}");
            RoundDirectorPatch.enemyParentList.Add(___EnemyParent);
            RoundDirectorPatch.enemyList.Add(__instance);
            RoundDirectorPatch.currentEnemyStatus.Add(EnemyStatus.Present);

            if(!levelEnemyNames.Contains(___EnemyParent.enemyName)) RoundDirectorPatch.levelEnemyNames.Add(___EnemyParent.enemyName);

            // This is doable because the carts are instantiated before the enemies.
            CartVocalPatch.AddEnemyRecordToAllCarts();
        }
    }
}
