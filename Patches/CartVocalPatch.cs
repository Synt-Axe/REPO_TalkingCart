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

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch(PhysGrabCart __instance, ref bool ___cartBeingPulled)
        {
            // Small carts are not included.
            if (__instance.isSmallCart) return;

            CartTalkingManager cart = __instance.GetComponent<CartTalkingManager>();
            if(cart == null)
            {
                Debug.Log("Creating CartTalkingManager instance on cart \"" + __instance.name + "\".");
                cart = SetupCart(__instance);
            }
            else if (!carts.Contains(cart))
            {
                Debug.Log("Re-added existing CartTalkingManager to carts list for cart \"" + __instance.name + "\".");
                carts.Add(cart);
            }
            cart.isCartBeingPulled = ___cartBeingPulled;
        }

        static CartTalkingManager SetupCart(PhysGrabCart physGrabCart)
        {
            CartTalkingManager cart = physGrabCart.gameObject.AddComponent<CartTalkingManager>();
            CartRoastSync cartRoastSync = physGrabCart.GetComponent<CartRoastSync>();
            if (cartRoastSync == null) cartRoastSync = physGrabCart.gameObject.AddComponent<CartRoastSync>();
            cart.cartRoastSync = cartRoastSync;
            cartRoastSync.cart = cart;
            carts.Add(cart);

            return cart;
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
    }
}
