using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkingCart.Patches
{
    [HarmonyPatch(typeof(EnemyHealth))]
    class EnemyHealthPatch
    {
        [HarmonyPatch("Death")]
        [HarmonyPrefix]
        static void EnemyDeathPatch(Enemy ___enemy)
        {
            if (ConfigManager.warnAboutEnemies.Value)
            {
                int enemyInd = RoundDirectorPatch.enemyList.IndexOf(___enemy);
                foreach (CartTalkingManager cart in CartVocalPatch.carts)
                {
                    cart.HandleDeadEnemy(enemyInd);
                }
            }
        }
    }
}
