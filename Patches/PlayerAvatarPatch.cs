using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkingCart.Patches
{
    [HarmonyPatch(typeof(PlayerAvatar))]
    class PlayerAvatarPatch
    {
        public static PlayerAvatar localPlayerAvatar; // Holds the player avatar of the main player. (The one controlling the machine this mod is on)
        public static PhysGrabber localPlayerPhysGrabber;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void StartPatch(PlayerAvatar __instance, ref bool ___isLocal)
        {
            if (___isLocal)
            {
                localPlayerAvatar = __instance;
                localPlayerPhysGrabber = __instance.GetComponent<PhysGrabber>();
            }
        }
    }
}
