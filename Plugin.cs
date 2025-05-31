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
        public static List<AudioClip> RoastsFX;
        public static List<string> RoastsText;
        internal static AssetBundle Bundle1;
        internal static AssetBundle Bundle2;

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
            ConfigManager.Initialize(((BaseUnityPlugin)this).Config);
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
            harmony.PatchAll(typeof(PhysGrabObjectPatch));

            SoundFX = new List<AudioClip>();
            string FolderLocation = Instance.Info.Location;
            FolderLocation = FolderLocation.TrimEnd("TalkingCart.dll".ToCharArray());
            Bundle1 = AssetBundle.LoadFromFile(FolderLocation + "talkingcartassetbundle");
            if (Bundle1 != null)
            {
                SoundFX = Bundle1.LoadAllAssets<AudioClip>().ToList();
                mls.LogInfo($"Audio Array Size: {SoundFX.Count}");
            }
            else mls.LogError("Failed to load asset bundle1 !!");

            RoastsFX = new List<AudioClip>();
            Bundle2 = AssetBundle.LoadFromFile(FolderLocation + "roasts");
            if (Bundle2 != null)
            {
                RoastsFX = Bundle2.LoadAllAssets<AudioClip>().ToList();
                mls.LogInfo($"Audio Array Size: {RoastsFX.Count}");
            }
            else mls.LogError("Failed to load asset bundle2 !!");

            RoastsText = new List<string> { "Nice one", "Who needs money anyways", "Keep at it and we'll have to sell you to pay rent", "Were you born like this?", "Yikes", "Who needs enemies when we have you", "When you were born the nurse took one look at you and then commited suicide", "The dumbass strikes again", "Wisdom chases you but you are faster", "A brain dead hippo would play better than this", "Are you kidding me right now?", "You're the type of guy who would eat all the supplies in a zombie movie and then die first", "I would rather burn my eyes looking at the sun than watch you play one more level", "You're so ugly if a school shooter saw you he would shoot himself instead", "Seriously can we kick this guy?", "Have you considered uninstalling the game?", "I bet your family doesn't invite you over for christmas", "I wish I had legs so I could kick your ass", "Single digit IQ", "This has to be some form of disability", "Good job", "Nice", "Dumbass", "Can't wait until they add the kick button", "Is your spirit animal a rock?", "You're so ugly even a TSA agent wouldn't touch you", "I'm considering retirement", "I should've taken the walmart job", "We should've left bro in the ship", "Great now go find the robe and give him a hug", "I would drop kick you if I had legs", "Bro is destroying items so much you'd think that he's 3 gnomes in a trenchcoat.", "This is why you're always picked last", "It takes a special person to screw up this badly", "This is why no one wants to join you", "This is why you're playing alone", "Oh nevermind it's just the idiot who keeps breaking shit"};

            mls.LogInfo($"Texts Array Size: {RoastsText.Count}");

        }
    }
}
