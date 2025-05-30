using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;

namespace TalkingCart.Patches
{
    [HarmonyPatch(typeof(PlayerController))]
    class PlayerControllerPatch
    {
        public static Vector3 playerPosition = new Vector3();

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch(PlayerController __instance)
        {
            playerPosition = __instance.transform.position;

            if(ChatManagerPatch.chatState != ChatManager.ChatState.Active) // Ignore input when player is writing in chat.
            {
                InputControl val1 = ((InputControl)Keyboard.current)[ConfigManager.communicateNearbyItemsKey.Value];
                if (((ButtonControl)val1).wasPressedThisFrame)
                {
                    CartTalkingManager cart = GetGrabbedCart();
                    if (cart != null) cart.CommunicateNearbyItems();
                }

                InputControl val2 = ((InputControl)Keyboard.current)[ConfigManager.toggleCommunicationsKey.Value];
                if (((ButtonControl)val2).wasPressedThisFrame)
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
    }
}
