using BepInEx;
using Strobotnik.Klattersynth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace TalkingCart.Patches
{
    public class CartTTSVoice : MonoBehaviour
    {
        Dictionary<string, AudioClip> TTSClipCache = new Dictionary<string, AudioClip>();

        public string pythonPath;
        public string scriptPath;
        public string audioOutputDir;

        public bool isBusy = false;

        void Awake()
        {
            string modDir = Path.Combine(Paths.PluginPath, "TalkingCart");
            pythonPath = Path.Combine(modDir, "python", "python.exe");
            scriptPath = Path.Combine(modDir, "tts_generator.py");
            audioOutputDir = Path.Combine(modDir, "audio_cache");

            // Create audio cache directory if it doesn't exist
            if (!Directory.Exists(audioOutputDir))
                Directory.CreateDirectory(audioOutputDir);
        }

        // Updated method to generate TTS and store the AudioClip
        public void GenerateTTSAndStore(string text)
        {
            StartCoroutine(GenerateTTSCoroutine(text));
            isBusy = true;
        }

        // Convenience method to get TTS clip from cache
        public AudioClip GetTTSClip(string text)
        {
            string hash = text.GetHashCode().ToString();
            if (TTSClipCache.ContainsKey(hash))
            {
                return TTSClipCache[hash];
            }
            return null;
        }

        IEnumerator GenerateTTSCoroutine(string text)
        {
            // Generate unique filename
            string hash = text.GetHashCode().ToString();
            string outputFile = Path.Combine(audioOutputDir, $"tts_{hash}.mp3");

            // Check if already cached in memory
            if (TTSClipCache.ContainsKey(hash))
            {
                //LastGeneratedTTSClip = TalkingCartBase.TTSClipCache[hash];
                isBusy = false;
                yield break;
            }

            // Check if file already exists (caching)
            if (!File.Exists(outputFile))
            {
                // Generate new TTS
                yield return StartCoroutine(GenerateTTSFile(text, outputFile));
            }

            // Load and store the audio
            yield return StartCoroutine(LoadAudioClip(outputFile, (clip) => {
                StoreAudioClip(clip, hash);
            }));
        }

        private void StoreAudioClip(AudioClip clip, string hash)
        {
            //LastGeneratedTTSClip = clip;
            TTSClipCache[hash] = clip;
            TalkingCartBase.mls.LogInfo($"Stored TTS AudioClip: {clip.name} (Cache size: {TTSClipCache.Count})");
            isBusy = false;
        }

        private IEnumerator GenerateTTSFile(string text, string outputFile)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = pythonPath,
                Arguments = $"\"{scriptPath}\" \"{text}\" \"{outputFile}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            Process process = Process.Start(startInfo);

            // Wait for process to complete (non-blocking)
            while (!process.HasExited)
            {
                yield return new WaitForSeconds(0.1f);
            }

            if (process.ExitCode != 0)
            {
                string error = process.StandardError.ReadToEnd();
                TalkingCartBase.mls.LogError($"TTS Generation failed: {error}");
            }

            process.Dispose();

            isBusy = false;
        }

        public IEnumerator LoadAudioClip(string filePath, System.Action<AudioClip> callback = null)
        {
            if (!File.Exists(filePath))
            {
                TalkingCartBase.mls.LogError($"Audio file not found: {filePath}");
                isBusy = false;
                yield break;
            }

            string url = "file://" + filePath.Replace("\\", "/");

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    clip.name = Path.GetFileNameWithoutExtension(filePath);
                    callback?.Invoke(clip);
                }
                else
                {
                    TalkingCartBase.mls.LogError($"Failed to load audio: {www.error}");
                }
            }
            isBusy = false;
        }

        // Method to clear TTS cache if needed
        public void ClearTTSCache()
        {
            foreach (var clip in TTSClipCache.Values)
            {
                if (clip != null)
                    Destroy(clip);
            }
            TTSClipCache.Clear();
            //LastGeneratedTTSClip = null;
            TalkingCartBase.mls.LogInfo("TTS Cache cleared");
        }
    }
}
