using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace TalkingCart.Patches
{
    [HarmonyPatch(typeof(RoundDirector))]
    class RoundDirectorPatch
    {
        public enum EnemyStatus
        {
            Present,
            Absent
        }

        public enum EnemyCommState
        {
            None,
            Nearby,
            Left,
            Despawned,
            Respawned,
            Dead
        }

        public static string[] enemyNames = new string[] { "Animal", "Banger", "Bowtie", "Chef", "Clown", "Apex Predator", "Gnome", "Headman", "Hidden", "Huntsman", "Mentalist", "Peeper", "Reaper", "Robe", "Rugrat", "Shadow Child", "Spewer", "Trudge", "Upscream" };

        public static string[] enemyNamesTextSingular = new string[] { "Animal", "Banger", "Bowtie", "Chef", "Clown", "Duck", "Gnome", "Headman", "Hidden", "Huntsman", "Mentalist", "Peeper", "Reaper", "Robe", "Rugrat", "Shadow Child", "Spewer", "Trudge", "Upscream" };
        public static string[] enemyNamesTextPlural = new string[] { "Animals", "Bangers", "Bowties", "Chefs", "Clowns", "Ducks", "Gnomes", "Headmen", "Hiddens", "Huntsmen", "Mentalists", "Peepers", "Reapers", "Robes", "Rugrats", "Shadow Children", "Spewers", "Trudges", "Upscreams" };

        public static bool initialEnemiesCommunicated = false;
        public static List<EnemyParent> enemyParentList = new List<EnemyParent>();
        public static List<Enemy> enemyList = new List<Enemy>();
        public static List<string> roundEnemyNamesList = new List<string>();
        public static List<EnemyStatus> currentEnemyStatus = new List<EnemyStatus>();

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        static void StartPatch()
        {
            // This runs when the level is changing.
            ValuableObjectsRecords.levelValuables.Clear();
            enemyParentList.Clear();
            enemyList.Clear();
            roundEnemyNamesList.Clear();
            currentEnemyStatus.Clear();
            initialEnemiesCommunicated = false;

            CartVocalPatch.carts.Clear();

            TalkingCartBase.mls.LogInfo("Resetting enemy lists!");
        }

        public static void AddEnemy(EnemyParent enemyParent, Enemy enemy)
        {
            enemyParentList.Add(enemyParent);
            enemyList.Add(enemy);
            currentEnemyStatus.Add(EnemyStatus.Present);

            if (!roundEnemyNamesList.Contains(enemyParent.enemyName)) roundEnemyNamesList.Add(enemyParent.enemyName);
        }
    }
}
