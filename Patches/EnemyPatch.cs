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
            RoundDirectorPatch.AddEnemy(___EnemyParent, __instance);
            // This is doable because the carts are instantiated before the enemies.
            CartVocalPatch.AddEnemyRecordToAllCarts();
        }
    }
}
