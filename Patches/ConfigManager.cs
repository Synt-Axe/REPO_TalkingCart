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
        // Controls
        public static ConfigEntry<string> communicateNearbyItemsKey;
        public static ConfigEntry<string> toggleCommunicationsKey;

        // Audio
        public static ConfigEntry<float> cartVoiceFalloffMultiplier;
        public static ConfigEntry<bool> alwaysUseGameTTSToVoiceCart;

        // Behaviour
        public static ConfigEntry<float> cartChanceToReactToDamagingItems;
        public static ConfigEntry<bool> warnAboutEnemies;

        public static void Initialize(ConfigFile cfg)
        {
            communicateNearbyItemsKey = cfg.Bind<string>("Controls", "CommunicateNearbyItemsKey", "x", "The key used to make the cart communicate how many valuables are nearby.");
            toggleCommunicationsKey = cfg.Bind<string>("Controls", "ToggleCommunicationsKey", "z", "The key used to toggle the cart's vocal communications (on/off).");

            cartVoiceFalloffMultiplier = cfg.Bind<float>("Audio", "CartVoiceFalloffMultiplier", 1.5f, "This number indicates how far away the player can hear the cart from.");
            alwaysUseGameTTSToVoiceCart = cfg.Bind<bool>("Audio", "AlwaysUseGameTTSToVoiceCart", false, "Turning this on will make the cart always communicate using the in-game TTS instead of Google's TTS. It's not recommended to turn this option on because the in-game tts can sound inaudible when voicing some of the voicelines.");

            cartChanceToReactToDamagingItems = cfg.Bind<float>("Behaviour", "CartChanceToReactToDamagingItems", 0.1f, "This number indicates how likely the cart is to roast a player if they damage or break an item next to it. Set to 0  if you want it disabled.");
            warnAboutEnemies = cfg.Bind<bool>("Behaviour", "WarnAboutEnemies", true, "When this is turned off, the cart will no longer warn the player about enemies.");
        }
    }
}
