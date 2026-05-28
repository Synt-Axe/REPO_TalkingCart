using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkingCart.Patches
{
    // This patch is used in development to make it easier to get the names of new enemies.

    [HarmonyPatch(typeof(EnemyDirector))]
    internal class EnemyDirectorPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        static void StartPatch(ref List<EnemySetup> ___enemiesDifficulty1, ref List<EnemySetup> ___enemiesDifficulty2, ref List<EnemySetup> ___enemiesDifficulty3)
        {
            Debug.Log("------------------------ enemiesDifficulty1 ------------------------");
            if (___enemiesDifficulty1 != null) PrintEnemies(___enemiesDifficulty1);
            else Debug.LogWarning("WARN: enemiesDifficulty1 is null");

            Debug.Log("------------------------ enemiesDifficulty2 ------------------------");
            if (___enemiesDifficulty2 != null) PrintEnemies(___enemiesDifficulty2);
            else Debug.LogWarning("WARN: enemiesDifficulty2 is null");

            Debug.Log("------------------------ enemiesDifficulty3 ------------------------");
            if (___enemiesDifficulty3 != null) PrintEnemies(___enemiesDifficulty3);
            else Debug.LogWarning("WARN: enemiesDifficulty3 is null");
        }

        static void PrintEnemies(List<EnemySetup> enemies)
        {
            foreach (EnemySetup enemy in enemies)
            {
                foreach (PrefabRef spawnObject in enemy.spawnObjects)
                {
                    EnemyParent enemyParent = spawnObject.Prefab.GetComponent<EnemyParent>();
                    if(enemyParent == null)
                    {
                        enemyParent = spawnObject.Prefab.GetComponentInChildren<EnemyParent>();
                    }

                    if (enemyParent != null)
                    {
                        string enemyName = enemyParent.enemyName;
                        int enemyInd = Array.IndexOf(RoundDirectorPatch.enemyNames, enemyName);
                        if(enemyInd == -1) // New enemy not in our lists.
                        {
                            Debug.Log("New Enemy: " + enemyName);
                        }
                    }
                }
            }
        }
    }
}
