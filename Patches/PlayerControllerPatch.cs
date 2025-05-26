using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

namespace TalkingCart.Patches
{
    [HarmonyPatch(typeof(PlayerController))]
    class PlayerControllerPatch
    {
        public static Vector3 playerPosition = new Vector3();

        public static CartTTSVoice cartTTSVoice;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void StartPatch()
        {
            cartTTSVoice = new GameObject().AddComponent<CartTTSVoice>();
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch(PlayerController __instance)
        {
            playerPosition = __instance.transform.position;

            if(ChatManagerPatch.chatState != ChatManager.ChatState.Active) // Ignore input when player is writing in chat.
            {
                if (Input.GetKeyDown(ConfigManager.communicateNearbyItemsKey.Value))
                {
                    CartTalkingManager cart = GetGrabbedCart();
                    if (cart != null) cart.CommunicateNearbyItems();
                }

                if (Input.GetKeyDown(ConfigManager.toggleCommunicationsKey.Value)) // Disable/enable cart communications.
                {
                    CartTalkingManager cart = GetGrabbedCart();
                    if (cart != null) cart.ToggleComms();
                }
            }
        }

        // Returns the cart that the player is grabbing.
        public static CartTalkingManager GetGrabbedCart()
        {
            if (PlayerAvatarPatch.localPlayerPhysGrabber.grabbedObjectTransform != null)
                return PlayerAvatarPatch.localPlayerPhysGrabber.grabbedObjectTransform.GetComponent<CartTalkingManager>();
            return null;
        }

        static string GetScriptableObjectFileName(ScriptableObject scriptableObject)
        {
            // Get the name directly from the object
            string objectName = scriptableObject.name;

            // In Unity, the name property of an asset typically matches its filename
            return objectName;
        }
    }
}
