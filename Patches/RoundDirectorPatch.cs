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

        public static string[] enemyNames = new string[] { "Animal", "Banger", "Bella", "Birthday Boy", "Bowtie", "Chef", "Cleanup Crew", "Clown", "Apex Predator", "Elsa", "Gambit", "Gnome", "Headgrab", "Headman", "Heart Hugger", "Hidden", "Huntsman", "Loom", "Mentalist", "Oogly", "Peeper", "Reaper", "Robe", "Rugrat", "Shadow Child", "Spewer", "Tick", "Trudge", "Upscream" };

        public static string[] enemyNamesTextSingular = new string[] { "Animal", "Banger", "Bella", "Birthday Boy", "Bowtie", "Chef", "Cleanup Crew", "Clown", "Duck", "Elsa", "Gambit", "Gnome", "Headgrab", "Headman", "Heart Hugger", "Hidden", "Huntsman", "Loom", "Mentalist", "Oogly", "Peeper", "Reaper", "Robe", "Rugrat", "Shadow Child", "Spewer", "Tick", "Trudge", "Upscream" };
        public static string[] enemyNamesTextPlural = new string[] { "Animals", "Bangers", "Bellas", "Birthday Boys", "Bowties", "Chefs", "Cleanup Crews", "Clowns", "Ducks", "Elsas", "Gambits", "Gnomes", "Headgrabs", "Headmen", "Heart Huggers", "Hiddens", "Huntsmen", "Looms", "Mentalists", "Ooglies", "Peepers", "Reapers", "Robes", "Rugrats", "Shadow Children", "Spewers", "Ticks", "Trudges", "Upscreams" };

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
            ValuableObjectsRecords.ResetLists();
            enemyParentList.Clear();
            enemyList.Clear();
            roundEnemyNamesList.Clear();
            currentEnemyStatus.Clear();
            initialEnemiesCommunicated = false;

            foreach (CartTalkingManager cart in CartVocalPatch.carts)
            {
                cart.lastCommunicatedStatus.Clear();
                cart.isEnemyNearby.Clear();
                cart.despawnRespawnTimer.Clear();
            }
            CartVocalPatch.carts.Clear();

            PlayerControllerPatch.playerPosition = new Vector3(99999f, 0f, 99999f); // Resetting the player posiition so it doesn't trigger the cart's level enemies communication early in the level.

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
