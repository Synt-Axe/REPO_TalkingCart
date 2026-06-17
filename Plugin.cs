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
        private const string modVersion = "1.5.2";

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
        public static int EnemyDiedInd = 32;
        public static int EnemyLeftInd = 61;
        public static int EnemyNamesPluralInd = 90;
        public static int EnemyNamesSingularInd = 119;
        public static int EnemyNearbyInd = 148;
        public static int EnemyRespawnedInd = 177;

        public static int NumbersInd = 206;


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

            //harmony.PatchAll(typeof(EnemyDirectorPatch)); // Used when looking for the new enemies names.

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

            RoastsText = new List<string> {
                "Nice one", "Who needs money anyways", "Keep at it and we'll have to sell you to pay rent", "Were you born like this?", "Yikes", "Who needs enemies when we have you", "The dumbass strikes again", "Wisdom chases you but you are faster", "A brain dead hippo would play better than this", "Are you kidding me right now?", "You're the type of guy who would eat all the supplies in a zombie movie and then die first", "I would rather burn my eyes looking at the sun than watch you play one more level", "Seriously can we kick this guy?", "Have you considered uninstalling the game?", "I bet your family doesn't invite you over for christmas", "I wish I had legs so I could kick your ass", "Single digit IQ", "This has to be some form of disability", "Good job", "Nice", "Dumbass", "Can't wait until they add the kick button", "Is your spirit animal a rock?", "You're so ugly even a TSA agent wouldn't touch you", "I'm considering retirement", "I should've taken the walmart job", "We should've left bro in the ship", "Great now go find the robe and give him a hug", "I would drop kick you if I had legs", "Bro is destroying items so much you'd think that he's 3 gnomes in a trenchcoat.", "This is why you're always picked last", "It takes a special person to screw up this badly", "This is why no one wants to join you", "This is why you're playing alone", "Oh nevermind it's just the idiot who keeps breaking shit",
                "I hope the shop has a discount on brain cells.", "I wish I could retire.", "You absolute walnut.", "This game has millions of players. Why am I stuck with this one?", "Keep it up", "You are the taxman's worst nightmare", "I did not sign up for this.", "I hate semibots", "You break more items than the rugrat", "Who needs the rugrat when we have you", "Please stop", "Nicely done! Now please stop.", "It's impossible to be this bad on purpose.", "Everyday you bring us further into debt.", "Everyday you bring us closer to bankruptcy", "Please leave me behind when you extract.", "At this point the monsters are the lesser threat.", "I hope the clown kicks you", "That was a prime example of your decision making skills", "You have the spatial awareness of a wet potato.", "I have met NPCs with better problem-solving skills", "I hate my job", "Who invited bro?", "Bro is suffering from a receding hairline and receding talent", "The only thing bigger than your forehead is your potential for chaos.", "That item cost more than your entire contribution to this team.", "Why are we still here? Just to suffer?", "You fumble items like you fumble life opportunities.", "A golden retriever could do your job with the bonus of not being difficult on the eyes.", "Ladies and gentlebots, we have an idiot."
            };

            mls.LogInfo($"Texts Array Size: {RoastsText.Count}");

        }
    }
}
