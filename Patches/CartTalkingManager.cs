using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ChatManager;
using static System.Net.Mime.MediaTypeNames;
using static TalkingCart.Patches.RoundDirectorPatch;

namespace TalkingCart.Patches
{
    class CartTalkingManager : MonoBehaviour
    {
        // Cart enabled bools.
        bool isCommEnabled = true;

        // Cart details.
        PhysGrabCart physGrabCart;
        public bool isCartBeingPulled = false;

        // Cart voicing variables.
        Sound cartSound = new Sound();
        Queue<AudioClip> cartVoiceQueue = new Queue<AudioClip>();
        Queue<float> cartVoiceDelayQueue = new Queue<float>();
        Queue<string> cartTextQueue = new Queue<string>();

        // Enemy variables.
        public List<EnemyCommState> lastCommunicatedStatus = new List<EnemyCommState>();
        public List<bool> isEnemyNearby = new List<bool>();
        public List<int> despawnRespawnTimer = new List<int>();

        float cartVoiceTimer = 0f; // These are used to stop the cart from starting a new sentence when it's already in the process of finishing a sentence.
        float checkTimer = 1f; // Timer for checking nearby enemies.

        // Cart talking text parameters.
        GameObject cartTalkText;
        RectTransform cartTalkTextTransform;
        TextMeshProUGUI cartTalkTextTMP;
        float textAlpha = 1f;
        float textAlphaTarget = 1f;
        float alphaCheckTimer = 0f;
        Color textColor = Color.white;

        // UI
        static TextMeshProUGUI enableInstructionTMP; // Static since all carts will share the same text ui.
        static TextMeshProUGUI itemsInstructionTMP;


        void InitializeVariables()
        {
            physGrabCart = GetComponent<PhysGrabCart>();
            isCartBeingPulled = false;

            cartSound = new Sound();
            cartVoiceQueue = new Queue<AudioClip>();
            cartVoiceDelayQueue = new Queue<float>();
            cartTextQueue = new Queue<string>();

            lastCommunicatedStatus = new List<EnemyCommState>();
            isEnemyNearby = new List<bool>();
            despawnRespawnTimer = new List<int>();

            cartVoiceTimer = 0f;
            checkTimer = 1f;
        }

        void Start()
        {
            InitializeVariables();

            // Setup cart sound source.
            Sound.CopySound(physGrabCart.soundHaulIncrease, cartSound);
            cartSound.Volume = 1f;
            cartSound.Pitch = 1f;
            cartSound.PitchRandom = 0f;

            // Initiate cart text.
            InitializeCartText();
        }

        void Update()
        {
            // Communicate the starter enemies if it's a non shop level and it's the first cart grab of the level.
            if (!RoundDirectorPatch.initialEnemiesCommunicated && SemiFunc.RunIsLevel())
            {
                Vector3 playerPos = new Vector3(PlayerControllerPatch.playerPosition.x, 0, PlayerControllerPatch.playerPosition.z);
                Vector3 cartPos = new Vector3(transform.position.x, 0, transform.position.z);
                float distanceToPlayer = Vector3.Distance(cartPos, playerPos);

                if(distanceToPlayer < 8f)
                {
                    RoundDirectorPatch.initialEnemiesCommunicated = true;
                    HandleStarterEnemies();
                }
            }

            // Run the cart voice queue script every frame.
            CartVoiceQueueRoller();

            // Check nearby enemies.
            checkTimer -= Time.deltaTime;
            if (checkTimer <= 0)
            {
                // Handle nearby enemies check.
                HandleNearbyEnemies();

                // Handle despawned and respawned enemies.
                HandleDespawnedEnemies();
                HandleRespawnedEnemies();

                checkTimer = 1f;
            }

            // Hanlding cart text.
            HandleCartText();
            HandleCartUI();
        }


        /***********************************/
        /*************CART TEXT*************/
        /***********************************/

        void InitializeCartText()
        {
            // Initiating with the TTS prefab.
            cartTalkText = Instantiate(WorldSpaceUIParent.instance.TTSPrefab, WorldSpaceUIParent.instance.transform.position, WorldSpaceUIParent.instance.transform.rotation, WorldSpaceUIParent.instance.transform); // TTS Prefab has no children. But has the following components: RectTransform, CanvasRenderer, WorldSpaceUITTS, TextMeshProUGUI

            // Destroying WorldSpaceUITTS component.
            WorldSpaceUITTS worldSpaceUITTS = cartTalkText.GetComponent<WorldSpaceUITTS>();
            if (worldSpaceUITTS != null)
                Destroy(worldSpaceUITTS);

            // Getting the needed components.
            cartTalkTextTransform = cartTalkText.GetComponent<RectTransform>();
            cartTalkTextTMP = cartTalkText.GetComponent<TextMeshProUGUI>();

            // Setting the text color and making it empty.
            cartTalkTextTMP.color = textColor;
            cartTalkTextTMP.text = "";
        }

        void HandleCartText()
        {
            // Handle position.
            Vector3 nextPosition = SemiFunc.UIWorldToCanvasPosition(transform.position);
            cartTalkTextTransform.anchoredPosition = nextPosition;

            // Handle text visibility.
            alphaCheckTimer -= Time.deltaTime;
            if (alphaCheckTimer <= 0f)
            {
                alphaCheckTimer = 0.1f;
                textAlphaTarget = 1f;

                float minDist = 5f;
                float maxDist = 25f;
                float playerDist = Vector3.Distance(transform.position, PlayerControllerPatch.playerPosition);
                if (playerDist > minDist)
                {
                    playerDist = Mathf.Clamp(playerDist, minDist, maxDist);
                    textAlphaTarget = 1f - (playerDist - minDist) / (maxDist - minDist);
                }
            }

            textAlpha = Mathf.Lerp(textAlpha, textAlphaTarget, 30f * Time.deltaTime);
            cartTalkTextTMP.color = new Color(textColor.r, textColor.g, textColor.b, textAlpha);
        }

        /**********************************************/
        /*************ENEMY COMMS HANDLING*************/
        /**********************************************/

        void HandleStarterEnemies()
        {
            // Communicate first sentence.
            EnqueueValues(TalkingCartBase.SoundFX[TalkingCartBase.LevelEnemiesVLInd], 0.2f, "Level enemies:");

            // Record number of enemies.
            int[] enemiesCount = new int[19];
            foreach (EnemyParent enemyParent in RoundDirectorPatch.enemyParentList)
            {
                int enemyNameInd = CartVocalPatch.GetEnemyNameIndex(enemyParent.enemyName);
                enemiesCount[enemyNameInd]++;
            }

            // Communicate enemies.
            for (int i = 0; i < enemiesCount.Length; i++)
            {
                int eCount = enemiesCount[i];
                if (eCount == 0)
                    continue;

                // If there is 1 of that enemy, no need to communicate a number.
                if (eCount == 1)
                {
                    int vlInd = TalkingCartBase.EnemyNamesSingularInd + i;
                    EnqueueValues(TalkingCartBase.SoundFX[vlInd], 0.2f, RoundDirectorPatch.enemyNamesTextSingular[i]);
                }
                else
                {
                    // This will for example write "4 Gnomes" twice instead of writing "4" alone and "Gnomes" alone.
                    string fullText = eCount.ToString() + " " + RoundDirectorPatch.enemyNamesTextPlural[i];

                    // Adding the number vl
                    int numberVLInd = TalkingCartBase.NumbersInd + eCount;
                    EnqueueValues(TalkingCartBase.SoundFX[numberVLInd], -0.2f, fullText);

                    // Adding the enemy name vl
                    int vlInd = TalkingCartBase.EnemyNamesPluralInd + i;
                    EnqueueValues(TalkingCartBase.SoundFX[vlInd], 0.2f, fullText);
                }
            }
        }

        void HandleNearbyEnemies()
        {
            Vector3 cartPosition = new Vector3(transform.position.x, 0f, transform.position.z);

            // Check nearby enemies.
            for (int i = 0; i < RoundDirectorPatch.enemyList.Count; i++)
            {
                EnemyParent enemyParentI = RoundDirectorPatch.enemyParentList[i];
                Enemy enemyI = RoundDirectorPatch.enemyList[i];
                EnemyStatus enemyStatusI = RoundDirectorPatch.currentEnemyStatus[i];

                int enemyNameInd = CartVocalPatch.GetEnemyNameIndex(enemyParentI.enemyName);
                if (enemyNameInd == 1 || enemyNameInd == 6) // Gnomes and Bangers
                    continue;

                // If enemy is despawned/dead, don't check.
                if (enemyStatusI == EnemyStatus.Absent)
                {
                    isEnemyNearby[i] = false;
                    continue;
                }

                // We get the enemy position.
                Vector3 enemyPosition = new Vector3(enemyI.transform.position.x, 0f, enemyI.transform.position.z);

                // We check the enemy distance.
                float distance = Vector3.Distance(cartPosition, enemyPosition);

                // If enemy is near and we haven't communicated that already.
                if (distance < 20f && !isEnemyNearby[i])
                {
                    TalkingCartBase.mls.LogInfo($"Enemy Nearby: {enemyParentI.enemyName}");
                    int voiceLineInd = TalkingCartBase.EnemyNearbyInd + enemyNameInd;
                    EnqueueValues(TalkingCartBase.SoundFX[voiceLineInd], 0.2f, RoundDirectorPatch.enemyNamesTextSingular[enemyNameInd] + " nearby");

                    isEnemyNearby[i] = true;
                    lastCommunicatedStatus[i] = EnemyCommState.Nearby;
                }
                else if (distance > 23f && isEnemyNearby[i]) // A 3 unit buffer so that the cart doesn't spam.
                {
                    TalkingCartBase.mls.LogInfo($"Enemy Left Area: {enemyParentI.enemyName}");
                    int voiceLineInd = TalkingCartBase.EnemyLeftInd + enemyNameInd;
                    EnqueueValues(TalkingCartBase.SoundFX[voiceLineInd], 0.2f, RoundDirectorPatch.enemyNamesTextSingular[enemyNameInd] + " left");

                    isEnemyNearby[i] = false;
                    lastCommunicatedStatus[i] = EnemyCommState.Nearby;
                }
            }
        }

        void HandleDespawnedEnemies()
        {
            // We loop through the enemies to see which has despawned.
            for (int i = 0; i < RoundDirectorPatch.enemyList.Count; i++)
            {
                // If this enemy is not absent, continue.
                if (RoundDirectorPatch.currentEnemyStatus[i] != EnemyStatus.Absent)
                    continue;
                // If this enemy's death/despawn was already communicated, continue.
                if (lastCommunicatedStatus[i] == EnemyCommState.Despawned || lastCommunicatedStatus[i] == EnemyCommState.Dead)
                    continue;

                if (despawnRespawnTimer[i] < 0) despawnRespawnTimer[i] = 0;
                despawnRespawnTimer[i] += 1;
                //TalkingCartBase.mls.LogInfo($"Enemy Despawn Counter {despawnRespawnTimer[i]}");

                if (despawnRespawnTimer[i] >= 3) // Enemy must remain despawned for 3 second before we communicate to player.
                {
                    EnemyParent enemyParentI = RoundDirectorPatch.enemyParentList[i];
                    TalkingCartBase.mls.LogInfo($"Enemy Despawned End: {enemyParentI.enemyName}");

                    int enemyNameInd = CartVocalPatch.GetEnemyNameIndex(enemyParentI.enemyName);
                    int voiceLineInd = TalkingCartBase.EnemyDespawnedInd + enemyNameInd;
                    EnqueueValues(TalkingCartBase.SoundFX[voiceLineInd], 0.2f, RoundDirectorPatch.enemyNamesTextSingular[enemyNameInd] + " despawned");

                    lastCommunicatedStatus[i] = EnemyCommState.Despawned;
                    despawnRespawnTimer[i] = 0;

                }
            }
        }

        void HandleRespawnedEnemies()
        {
            // We loop through the enemies to see which has respawned.
            for (int i = 0; i < RoundDirectorPatch.enemyList.Count; i++)
            {
                // If this enemy is absent, continue.
                if (RoundDirectorPatch.currentEnemyStatus[i] != EnemyStatus.Present)
                    continue;
                // If the last communicated state is anything other than despawned/dead, continue.
                if (lastCommunicatedStatus[i] != EnemyCommState.Despawned && lastCommunicatedStatus[i] != EnemyCommState.Dead)
                    continue;

                if (despawnRespawnTimer[i] > 0) despawnRespawnTimer[i] = 0;
                despawnRespawnTimer[i] -= 1;
                //TalkingCartBase.mls.LogInfo($"Enemy Respawn Counter {despawnRespawnTimer[i]}");

                if (despawnRespawnTimer[i] <= -3) // Enemy must remain respawned for 3 second before we communicate to player.
                {
                    EnemyParent enemyParentI = RoundDirectorPatch.enemyParentList[i];
                    TalkingCartBase.mls.LogInfo($"Enemy Respawned End: {enemyParentI.enemyName}");

                    int enemyNameInd = CartVocalPatch.GetEnemyNameIndex(enemyParentI.enemyName);
                    int voiceLineInd = TalkingCartBase.EnemyRespawnedInd + enemyNameInd;
                    EnqueueValues(TalkingCartBase.SoundFX[voiceLineInd], 0.2f, RoundDirectorPatch.enemyNamesTextSingular[enemyNameInd] + " respawned");

                    lastCommunicatedStatus[i] = EnemyCommState.Respawned;
                    despawnRespawnTimer[i] = 0;

                }
            }
        }

        public void HandleDeadEnemy(int enemyInd)
        {
            // If this enemy's death/despawn was already communicated, return.
            if (lastCommunicatedStatus[enemyInd] == EnemyCommState.Despawned || lastCommunicatedStatus[enemyInd] == EnemyCommState.Dead)
            {
                TalkingCartBase.mls.LogError("Enemy death ignored!");
                return;
            }

            EnemyParent enemyParent = RoundDirectorPatch.enemyParentList[enemyInd];
            TalkingCartBase.mls.LogInfo($"Enemy Died End: {enemyParent.enemyName}");
            int enemyNameInd = CartVocalPatch.GetEnemyNameIndex(enemyParent.enemyName);
            if (enemyNameInd == 1 || enemyNameInd == 6) // Gnomes and Bangers
                return;

            // Adding the voice to queue.
            int voiceLineInd = TalkingCartBase.EnemyDiedInd + enemyNameInd;
            EnqueueValues(TalkingCartBase.SoundFX[voiceLineInd], 0.2f, RoundDirectorPatch.enemyNamesTextSingular[enemyNameInd] + " died");

            lastCommunicatedStatus[enemyInd] = EnemyCommState.Dead;
        }


        /*******************************/
        /*************Items*************/
        /*******************************/

        public void CommunicateNearbyItems()
        {
            int valuablesNearbyCount = 0;
            Vector3 cartPosition = new Vector3(transform.position.x, 0f, transform.position.z);

            foreach (ValuableObject valobj in ValuableObjectsRecords.levelValuables)
            {
                if (valobj == null)
                    continue;

                Vector3 valuablePosition = new Vector3(valobj.transform.position.x, 0f, valobj.transform.position.z);

                // We check the valuable distance.
                float distance = Vector3.Distance(cartPosition, valuablePosition);

                if (distance > 1.1f && distance < 20f)
                {
                    valuablesNearbyCount++;
                }
            }

            TalkingCartBase.mls.LogInfo($"Valuables Nearby: {valuablesNearbyCount}");

            // Adding the VLs to the queue.
            // This will for example write "4 Items" twice instead of writing "4" alone and "Items" alone.
            string fullText = valuablesNearbyCount.ToString() + " items nearby";

            // Adding the number vl
            int numberVLInd = TalkingCartBase.NumbersInd + valuablesNearbyCount;
            EnqueueValues(TalkingCartBase.SoundFX[numberVLInd], -0.2f, fullText);

            // Adding the enemy name vl
            int vlInd = TalkingCartBase.ItemsNearbyVLInd;
            EnqueueValues(TalkingCartBase.SoundFX[vlInd], 0.2f, fullText);
        }


        /*******************************/
        /*************QUEUE*************/
        /*******************************/

        void EnqueueValues(AudioClip audioClip, float duration, string text)
        {
            // If comms is off, return.
            if (!isCommEnabled) return;

            cartVoiceQueue.Enqueue(audioClip);
            cartVoiceDelayQueue.Enqueue(duration);
            cartTextQueue.Enqueue(text);
        }

        void CartVoiceQueueRoller()
        {
            cartVoiceTimer -= Time.deltaTime;
            // Clearing text after queue is done.
            if (cartVoiceQueue.Count == 0 && cartVoiceTimer <= 0 && cartTalkTextTMP.text != "") cartTalkTextTMP.text = "";
            // Check if the queue is not empty.
            if (cartVoiceQueue.Count == 0) return;
            // Check that no sound is currently playing.
            if (cartVoiceTimer > 0) return;
            // If communications is off, return.
            if (!isCommEnabled) return;

            //TalkingCartBase.mls.LogInfo("Voicing Sound");

            // Say the enemy name.
            AudioClip nextClip = cartVoiceQueue.Dequeue();
            float delay = cartVoiceDelayQueue.Dequeue();
            cartSound.Sounds = new AudioClip[] { nextClip };
            cartVoiceTimer = nextClip.length + delay; // Used to add a buffer between sentences.
            cartSound.Play(physGrabCart.displayText.transform.position);

            // Set the cart text.
            string text = cartTextQueue.Dequeue();
            cartTalkTextTMP.text = text;
        }


        /*********************************************/
        /*************ENABLE/DISABLE CART*************/
        /*********************************************/

        public void ToggleComms()
        {
            isCommEnabled = !isCommEnabled;

            if (!isCommEnabled)
            {
                // Clear the queues.
                cartVoiceQueue.Clear();
                cartVoiceDelayQueue.Clear();
                cartTextQueue.Clear();
            }

            TalkingCartBase.mls.LogInfo($"Is comm enabled: {isCommEnabled}");
        }

        Transform CreateInstructionUI()
        {
            // Getting the HUD parent.
            Transform hudCanvas = GameObject.FindObjectsOfType<Canvas>()[1].transform;
            Transform gameHudObject = hudCanvas.GetChild(0).GetChild(0); // This is where all the screen ui is kept.

            // Creating the instruction text ui as a copy from the extraction count ui text.
            Transform instructionUI = GameObject.Instantiate(gameHudObject.GetChild(0), gameHudObject);

            // Removing the unnecessary script.
            GoalUI goalUI = instructionUI.GetComponent<GoalUI>();
            if (goalUI != null) GameObject.Destroy(goalUI);

            // Removing scanlines.
            Destroy(instructionUI.GetChild(0).gameObject);

            return instructionUI;
        }

        void HandleCartUI()
        {
            // If the cart is not being handled by the player and no other cart is being handled by the player either, we hide the text. (If it's already initiated and it's not already hidden)
            if (PlayerControllerPatch.GetGrabbedCart() != this)
            {
                if (enableInstructionTMP != null)
                {
                    if (enableInstructionTMP.text != "" && PlayerControllerPatch.GetGrabbedCart() == null) enableInstructionTMP.text = "";
                }
                if (itemsInstructionTMP != null)
                {
                    if (itemsInstructionTMP.text != "" && PlayerControllerPatch.GetGrabbedCart() == null) itemsInstructionTMP.text = "";
                }
                return;
            }

            // If the cart is being handdled by the player, but the ui is not initialized yet, we initialize it.
            if (enableInstructionTMP == null)
            {
                Transform instructionUI = CreateInstructionUI();

                // Getting the relevant components.
                RectTransform instructionRectTransform = instructionUI.GetComponent<RectTransform>();
                enableInstructionTMP = instructionUI.GetComponent<TextMeshProUGUI>();

                // Customizing the text.
                instructionRectTransform.anchoredPosition = new Vector2(-4f, -70f);
                enableInstructionTMP.color = Color.white;
                enableInstructionTMP.fontSize = 20;
            }
            // If the cart is being handdled by the player, but the ui is not initialized yet, we initialize it.
            if (itemsInstructionTMP == null)
            {
                Transform instructionUI = CreateInstructionUI();

                // Getting the relevant components.
                RectTransform instructionRectTransform = instructionUI.GetComponent<RectTransform>();
                itemsInstructionTMP = instructionUI.GetComponent<TextMeshProUGUI>();

                // Customizing the text.
                instructionRectTransform.anchoredPosition = new Vector2(-4f, -100f);
                itemsInstructionTMP.color = Color.white;
                itemsInstructionTMP.fontSize = 20;
            }

            // Set the ui text for enable/disable instruction.
            if (isCommEnabled)
            {
                if (enableInstructionTMP.text != $"Click '{ConfigManager.toggleCommunicationsKey.Value}' to disable cart voice.") enableInstructionTMP.text = $"Click '{ConfigManager.toggleCommunicationsKey.Value}' to disable cart voice.";
            }
            else if (enableInstructionTMP.text != $"Click '{ConfigManager.toggleCommunicationsKey.Value}' to enable cart voice.") enableInstructionTMP.text = $"Click '{ConfigManager.toggleCommunicationsKey.Value}' to enable cart voice.";
            // Set the ui text for items instruction.
            if (itemsInstructionTMP.text != $"Click '{ConfigManager.communicateNearbyItemsKey.Value}' for items count.") itemsInstructionTMP.text = $"Click '{ConfigManager.communicateNearbyItemsKey.Value}' for items count.";
        }
    }
}
