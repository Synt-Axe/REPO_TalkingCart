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

        public static void Initialize(ConfigFile cfg)
        {
            communicateNearbyItemsKey = cfg.Bind<string>("Controls", "CommunicateNearbyItemsKey", "x", "The key used to make the cart communicate how many valuables are nearby.");
            toggleCommunicationsKey = cfg.Bind<string>("Controls", "ToggleCommunicationsKey", "z", "The key used to toggle the cart's vocal communications (on/off).");
        }
    }
}
