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
using SelfMovingCart.Patches;

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

                if (WasButtonPressedThisFrame(ConfigManager.communicateNearbyItemsKey.Value))
                {
                    CartTalkingManager cart = GetGrabbedCart();
                    if (cart != null) cart.CommunicateNearbyItems();
                }

                if (WasButtonPressedThisFrame(ConfigManager.toggleCommunicationsKey.Value))
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

        static List<string> mouseBtns = new List<string>() { "leftButton", "rightButton", "middleButton", "forwardButton", "backButton" };
        static bool WasButtonPressedThisFrame(string btn)
        {
            InputControl val;
            if (mouseBtns.Contains(btn))
            {
                val = ((InputControl)Mouse.current)[btn];
            } else
            {
                val = ((InputControl)Keyboard.current)[btn];
            }

            return ((ButtonControl)val).wasPressedThisFrame;
        }
    }
}
