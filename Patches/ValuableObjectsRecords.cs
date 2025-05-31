using HarmonyLib;
using SelfMovingCart.Patches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkingCart.Patches
{
    [HarmonyPatch(typeof(ValuableObject))]
    class ValuableObjectsRecords
    {
        public static List<ValuableObject> levelValuables = new List<ValuableObject>();
        public static List<float> valuableLastValue = new List<float>();
        public static List<bool> isValidForRoast = new List<bool>();
        public static List<Coroutine> roastValidityCoroutines = new List<Coroutine>();

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void ValuableStartPatch(ValuableObject __instance)
        {
            levelValuables.Add(__instance);
            isValidForRoast.Add(false);
            roastValidityCoroutines.Add(null);
            valuableLastValue.Add(100); // All valuables are worth 100 at the start. This value is changed in the first update.
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch(ValuableObject __instance)
        {
            int ind = levelValuables.IndexOf(__instance);
            float dollarValueCurrent = ReflectionHelper.GetPrivateField<float>(__instance, "dollarValueCurrent");
            if (dollarValueCurrent < valuableLastValue[ind])
            {
                // Damaged.
                CartTalkingManager closestCart = CartVocalPatch.carts[0];
                float closestDist = Mathf.Infinity;
                foreach(CartTalkingManager cart in CartVocalPatch.carts)
                {
                    float dist = Vector3.Distance(cart.transform.position, __instance.transform.position);
                    if (dist < closestDist)
                    {
                        closestCart = cart;
                        closestDist = dist;
                    }
                }
                if(closestDist <= 12f && isValidForRoast[ind])
                {
                    closestCart.cartRoastSync.AttemptRoast();
                }
            }
            // Keeping track of value in each frame.
            if(dollarValueCurrent != valuableLastValue[ind])
            {
                valuableLastValue[ind] = dollarValueCurrent;
            }
        }

        public static void ResetLists()
        {
            levelValuables.Clear();
            isValidForRoast.Clear();
            roastValidityCoroutines.Clear();
            valuableLastValue.Clear();
        }
    }
}
