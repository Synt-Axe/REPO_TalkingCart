using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkingCart.Patches
{
    class ConfigManager
    {
        public static ConfigEntry<string> communicateNearbyItemsKey;
        public static ConfigEntry<string> toggleCommunicationsKey;
        public static ConfigEntry<float> cartVoiceFalloffMultiplier;
        public static ConfigEntry<float> cartChanceToReactToDamagingItems;

        public static void Initialize(ConfigFile cfg)
        {
            communicateNearbyItemsKey = cfg.Bind<string>("Controls", "CommunicateNearbyItemsKey", "x", "The key used to make the cart communicate how many valuables are nearby.");
            toggleCommunicationsKey = cfg.Bind<string>("Controls", "ToggleCommunicationsKey", "z", "The key used to toggle the cart's vocal communications (on/off).");

            cartVoiceFalloffMultiplier = cfg.Bind<float>("Audio", "CartVoiceFalloffMultiplier", 1.5f, "This number indicates how far away the player can hear the cart from.");

            cartChanceToReactToDamagingItems = cfg.Bind<float>("Behaviour", "CartChanceToReactToDamagingItems", 0.1f, "This number indicates how likely the cart is to roast a player if they damage or break an item next to it. Set to 0  if you want it disabled.");
        }
    }
}
