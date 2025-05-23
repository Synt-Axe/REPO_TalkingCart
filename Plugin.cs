using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkingCart.Patches;
using UnityEngine;

namespace TalkingCart
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class TalkingCartBase : BaseUnityPlugin
    {
        private const string modGUID = "Syntaxe.TalkingCart";
        private const string modName = "Talking Cart";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static TalkingCartBase Instance;

        public static ManualLogSource mls;

        public static List<AudioClip> SoundFX;
        internal static AssetBundle Bundle;

        public static int AllEnemiesNearbyVLInd = 0;
        public static int ItemsNearbyVLInd = 1;
        public static int LevelEnemiesVLInd = 2;

        public static int EnemyDespawnedInd = 3;
        public static int EnemyDiedInd = 22;
        public static int EnemyLeftInd = 41;
        public static int EnemyNamesPluralInd = 60;
        public static int EnemyNamesSingularInd = 79;
        public static int EnemyNearbyInd = 98;
        public static int EnemyRespawnedInd = 117;

        public static int NumbersInd = 136;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo($"{modGUID} is now awake!");

            harmony.PatchAll(typeof(TalkingCartBase));

            harmony.PatchAll(typeof(ValuableObjectsRecords));
            harmony.PatchAll(typeof(RoundDirectorPatch));
            harmony.PatchAll(typeof(EnemyPatch));
            harmony.PatchAll(typeof(EnemyHealthPatch));
            harmony.PatchAll(typeof(EnemyParentPatch));
            harmony.PatchAll(typeof(PlayerControllerPatch));
            harmony.PatchAll(typeof(CartVocalPatch));
            harmony.PatchAll(typeof(ChatManagerPatch));
            harmony.PatchAll(typeof(PlayerAvatarPatch));
            

            SoundFX = new List<AudioClip>();
            string FolderLocation = Instance.Info.Location;
            FolderLocation = FolderLocation.TrimEnd("TalkingCart.dll".ToCharArray());
            Bundle = AssetBundle.LoadFromFile(FolderLocation + "talkingcartassetbundle");
            if (Bundle != null)
            {
                SoundFX = Bundle.LoadAllAssets<AudioClip>().ToList();
                mls.LogInfo($"Audio Array Size: {SoundFX.Count}");
            }
            else
            {
                mls.LogError("Failed to load asset bundle!!");
            }
        }
    }
}
