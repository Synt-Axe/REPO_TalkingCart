using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkingCart.Patches
{
    [HarmonyPatch(typeof(PhysGrabObject))]
    class PhysGrabObjectPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch(PhysGrabObject __instance, ref bool ___grabbed, ref bool ___isValuable)
        {
            if (___isValuable)
            {
                ValuableObject valuableObject = __instance.GetComponent<ValuableObject>();
                int ind = ValuableObjectsRecords.levelValuables.IndexOf(valuableObject);
                if (___grabbed)
                {
                    if (ValuableObjectsRecords.roastValidityCoroutines[ind] != null) __instance.StopCoroutine(ValuableObjectsRecords.roastValidityCoroutines[ind]);
                    ValuableObjectsRecords.isValidForRoast[ind] = true;
                }
                else
                {
                    if (ValuableObjectsRecords.isValidForRoast[ind] && ValuableObjectsRecords.roastValidityCoroutines[ind] == null)
                    {
                        ValuableObjectsRecords.roastValidityCoroutines[ind] = __instance.StartCoroutine(DisableRoastValidity(ind));
                    }
                }
            }
        }

        static IEnumerator DisableRoastValidity(int itemInd)
        {
            yield return new WaitForSeconds(2f);
            ValuableObjectsRecords.isValidForRoast[itemInd] = false;
            ValuableObjectsRecords.roastValidityCoroutines[itemInd] = null;
        }
    }
}
