﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppLirp;
using MelonLoader;
using UnityEngine;


namespace AudioReplacerMod
{

    public class ModManager : MelonMod
    {
        public static bool isDebugActivated;
        public static ModManager instance;
        private AssetManager assetManager;
        private static List<string> processedRider = new List<string>();

        public override void OnInitializeMelon()
        {
            instance = this;
            isDebugActivated = false;

            assetManager = new AssetManager();
            assetManager.Init();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "Loader")
            {
                assetManager.SetupData();
                MelonLogger.Msg("AudioReplacer mod initialized.");
            }
        }

        public static void ReplaceRiderSounds(GameObject in_rider)
        {
            if (processedRider.Contains(in_rider.name))
            {
                ModLogger.Log($"Rider {in_rider.name} already processed");
                return;
            }
            else
            {
                ModLogger.Log($"Replacing sounds for {in_rider.name}");
                processedRider.Add(in_rider.name);
                SnowboardSounds snowboardSounds = in_rider.GetComponent<SnowboardSounds>();
                if (snowboardSounds)
                {
                    ModLogger.Log("Sound component found");
                    ReplaceAudioMaterialData(snowboardSounds.bigPipeClips, MaterialClip.BigPipes);
                    ReplaceAudioMaterialData(snowboardSounds.concreteClips, MaterialClip.Concrete);
                    ReplaceAudioMaterialData(snowboardSounds.courseClips, MaterialClip.Course);
                    ReplaceAudioMaterialData(snowboardSounds.deepSnowClips, MaterialClip.DeepSnow);
                    ReplaceAudioMaterialData(snowboardSounds.woodClips, MaterialClip.JibbingBox);
                    ReplaceAudioMaterialData(snowboardSounds.railClips, MaterialClip.Rails);
                    ReplaceAudioMaterialData(snowboardSounds.smallRailClips, MaterialClip.SmallRail);
                    ReplaceAudioMaterialData(snowboardSounds.softFenceClips, MaterialClip.SoftFence);

                    ModLogger.Log($"End of sounds replacement for rider {in_rider.name}");
                }
                else ModLogger.Log("Sound component not found");
            }
        }

        public static void ReplaceAudioMaterialData(SnowboardSounds.MaterialClips materialClips, MaterialClip materialClip)
        {
            AudioProperties _audioProperties;

            _audioProperties = instance.assetManager.GetAudioProperties(materialClip, AssetType.Forward);
            if (_audioProperties.audioClips.Count > 0)
            {
                ModLogger.Log("Replacing properties...");
                materialClips.Forward = _audioProperties.audioClips[0];
                materialClips.forwardMinVolume = _audioProperties.volume;
                materialClips.ForwardVolume = _audioProperties.volume;
                materialClips.minForwardPitch = _audioProperties.pitchBase;
                materialClips.ForwardPitch = _audioProperties.pitchVar;
                ModLogger.Log("Properties replaced!");
            }
            else ModLogger.Log("Not replacing properties for this asset type");

            _audioProperties = instance.assetManager.GetAudioProperties(materialClip, AssetType.Turn);
            if (_audioProperties.audioClips.Count > 0)
            {
                ModLogger.Log("Replacing properties...");
                materialClips.Turn = _audioProperties.audioClips[0];
                materialClips.TurnVolume = _audioProperties.volume;
                materialClips.minTurnPitch = _audioProperties.pitchBase;
                materialClips.TurnPitch = _audioProperties.pitchVar;
                ModLogger.Log("Properties replaced!");
            }
            else ModLogger.Log("Not replacing properties for this asset type");

            _audioProperties = instance.assetManager.GetAudioProperties(materialClip, AssetType.LandingHard);
            if (_audioProperties.audioClips.Count > 0)
            {
                ModLogger.Log("Replacing properties...");
                List<SnowboardSounds.AudioClipSettings> landingSettings = new List<SnowboardSounds.AudioClipSettings>();
                for (int i = 0; i < _audioProperties.audioClips.Count; i++)
                {
                    SnowboardSounds.AudioClipSettings audioClipSettings = new SnowboardSounds.AudioClipSettings();
                    audioClipSettings.clip = _audioProperties.audioClips[i];
                    audioClipSettings.volume = _audioProperties.volume;
                    landingSettings.Add(audioClipSettings);
                }
                materialClips.LandClipsHard = landingSettings.ToArray();
                ModLogger.Log("Properties replaced!");
            }
            else ModLogger.Log("Not replacing properties for this asset type");

            _audioProperties = instance.assetManager.GetAudioProperties(materialClip, AssetType.LandingSoft);
            if (_audioProperties.audioClips.Count > 0)
            {
                ModLogger.Log("Replacing properties...");
                List<SnowboardSounds.AudioClipSettings> landingSettings = new List<SnowboardSounds.AudioClipSettings>();
                for (int i = 0; i < _audioProperties.audioClips.Count; i++)
                {
                    SnowboardSounds.AudioClipSettings audioClipSettings = new SnowboardSounds.AudioClipSettings();
                    audioClipSettings.clip = _audioProperties.audioClips[i];
                    audioClipSettings.volume = _audioProperties.volume;
                    landingSettings.Add(audioClipSettings);
                }
                materialClips.LandClipsSoft = landingSettings.ToArray();
                ModLogger.Log("Properties replaced!");
            }
            else ModLogger.Log("Not replacing properties for this asset type");
        }
    }
}
