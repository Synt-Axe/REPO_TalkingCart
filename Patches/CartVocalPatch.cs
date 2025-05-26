using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TalkingCart.Patches.RoundDirectorPatch;
using UnityEngine;
using TMPro;

namespace TalkingCart.Patches
{
    [HarmonyPatch(typeof(PhysGrabCart))]
    class CartVocalPatch
    {
        public static List<CartTalkingManager> carts = new List<CartTalkingManager>();
        static List<PhysGrabObject> cartsPhysGrabObjects = new List<PhysGrabObject>(); // TODO clear it in Round Director.

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void StartPatch(PhysGrabCart __instance, ref PhysGrabObject ___physGrabObject)
        {
            // Small carts are not included.
            if (__instance.isSmallCart) return;

            CartTalkingManager cart = __instance.gameObject.AddComponent<CartTalkingManager>();
            carts.Add(cart);

            cartsPhysGrabObjects.Add(___physGrabObject);
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch(PhysGrabCart __instance, ref bool ___cartBeingPulled)
        {
            // Small carts are not included.
            if (__instance.isSmallCart) return;

            CartTalkingManager cart = __instance.GetComponent<CartTalkingManager>();
            cart.isCartBeingPulled = ___cartBeingPulled;
        }

        public static void AddEnemyRecordToAllCarts()
        {
            foreach (CartTalkingManager cart in carts)
            {
                cart.isEnemyNearby.Add(false);
                cart.lastCommunicatedStatus.Add(EnemyCommState.None);
                cart.despawnRespawnTimer.Add(0);
            }
        }

        static int GetNearestCartToPlayer()
        {
            int nearestInd = 0;
            float nearestDist = Mathf.Infinity;

            for(int i = 0; i < carts.Count; i++)
            {
                float dist = Vector3.Distance(PlayerControllerPatch.playerPosition, carts[i].transform.position);
                if(dist < nearestDist)
                {
                    nearestInd = i;
                    nearestDist = dist;
                }
            }

            return nearestInd;
        }

        public static int GetEnemyNameIndex(string enemyName)
        {
            // Get enemy name index.
            int enemyNameInd = RoundDirectorPatch.enemyNames.IndexOf(enemyName);
            if (enemyNameInd == -1)
            {
                TalkingCartBase.mls.LogError($"Adding new enemy"); // TODO
                return 18;
            }
            return enemyNameInd;
        }
    }
}
