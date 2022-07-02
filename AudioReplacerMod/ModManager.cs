using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lirp;
using MelonLoader;
using UnhollowerRuntimeLib;
using UnityEngine;


namespace AudioReplacerMod
{

    public class ModManager : MelonMod
    {
        public static bool isDebugActivated;
        public static UserSession userSession;
        public static PlayerRigReplayManager replayManager;
        public static ModManager instance;
        private AssetManager assetManager;

        public static List<GameObject> gameplayRider = new List<GameObject>();
        public static GameObject replayRider;
        public static SnowboardSounds gameplaySnowboardSounds;
        public static SnowboardSounds replaySnowboardSounds;
        public static bool areGameplayRidersRegistered;

        public override void OnApplicationStart()
        {
            instance = this;
            isDebugActivated = true;

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

            if (sceneName == "GameBase")
            {
                ModLogger.Log("Try get UserSession");
                userSession = GameObject.Find("UserSession").GetComponent<UserSession>();
                ModLogger.Log("UserSession found");
            }

            if (sceneName == "mountain01Logic")
            {
                ModLogger.Log("Try get ReplayManager");
                replayManager = GameObject.Find("ReplayManager").GetComponent<PlayerRigReplayManager>();
                ModLogger.Log("ReplayManager found");
            }
        }

        public override void OnLateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                MaterialClipsInfo _materialClipInfo;
                assetManager.materialClipsDict.TryGetValue(MaterialClip.Course, out _materialClipInfo);
                ModLogger.Log(assetManager.GetAudioPropertiesAsString(assetManager.GetAudioProperties(MaterialClip.Course, AssetType.Forward)));
                //if (_materialClipInfo.forwardInfo.isReplaced)
                //  gameplaySnowboardSounds.courseClips.Forward = _materialClipInfo.forwardInfo.audioClips[0];
            }

            if (Input.GetKeyDown(KeyCode.Keypad1))
                ModLogger.Log(assetManager.GetAudioProperties(MaterialClip.Course, AssetType.Forward).audioClips[0].name);
        }

        public static void ReplaceGameplayRiderSounds(GameObject in_rider)
        {
            /*
            ModLogger.Log($"Replacing sounds for {gameplayRider.Count} riders");
            foreach(GameObject rider in gameplayRider)
            {
                ModLogger.Log($"Checking sound component for rider {rider.name}");
                SnowboardSounds snowboardSounds = rider.GetComponent<SnowboardSounds>();
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
                }
                else ModLogger.Log("Sound component not found");

                if (rider.name != "Snowboarder2(Clone)")
                    rider.SetActive(false);
            }
            ModLogger.Log("End of riders sounds replacing for all riders");
            */
            ModLogger.Log($"Replacing sounds for {in_rider}");
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

        public static void ReplaceReplayRiderSounds()
        {

        }

        public static void ReplaceAudioMaterialData(SnowboardSounds.MaterialClips materialClips, MaterialClip materialClip)
        {
            AudioProperties _audioProperties;

            _audioProperties = instance.assetManager.GetAudioProperties(materialClip, AssetType.Forward);
            if (_audioProperties.isReplaced)
            {
                ModLogger.Log("Replacing properties...");
                materialClips.Forward = _audioProperties.audioClips[0];
                materialClips.forwardMinVolume = _audioProperties.volumeMin;
                materialClips.ForwardVolume = _audioProperties.volumeMax;
                materialClips.minForwardPitch = _audioProperties.pitchMin;
                materialClips.ForwardPitch = _audioProperties.pitchMax;
                ModLogger.Log("Properties replaced!");
            }
            else ModLogger.Log("Not replacing properties for this asset type");

            _audioProperties = instance.assetManager.GetAudioProperties(materialClip, AssetType.Turn);
            if (_audioProperties.isReplaced)
            {
                ModLogger.Log("Replacing properties...");
                materialClips.Turn = _audioProperties.audioClips[0];
                materialClips.TurnVolume = _audioProperties.volumeMax;
                materialClips.minTurnPitch = _audioProperties.pitchMin;
                materialClips.TurnPitch = _audioProperties.pitchMax;
                ModLogger.Log("Properties replaced!");
            }
            else ModLogger.Log("Not replacing properties for this asset type");

            _audioProperties = instance.assetManager.GetAudioProperties(materialClip, AssetType.LandingHard);
            if (_audioProperties.isReplaced)
            {
                ModLogger.Log("Replacing properties...");
                List<SnowboardSounds.AudioClipSettings> landingSettings = materialClips.LandClipsHard.ToList(); 
                for (int i = 0; i < landingSettings.Count; i++)
                {
                    landingSettings[i].clip = _audioProperties.audioClips[i];
                    landingSettings[i].volume = _audioProperties.volumeMax;
                }
                ModLogger.Log("Properties replaced!");
            }
            else ModLogger.Log("Not replacing properties for this asset type");

            _audioProperties = instance.assetManager.GetAudioProperties(materialClip, AssetType.LandingSoft);
            if (_audioProperties.isReplaced)
            {
                ModLogger.Log("Replacing properties...");
                List<SnowboardSounds.AudioClipSettings> landingSettings = materialClips.LandClipsSoft.ToList();
                for (int i = 0; i < landingSettings.Count; i++)
                {
                    landingSettings[i].clip = _audioProperties.audioClips[i];
                    landingSettings[i].volume = _audioProperties.volumeMax;
                }
                ModLogger.Log("Properties replaced!");
            }
            else ModLogger.Log("Not replacing properties for this asset type");
        }

        public static GameObject FindInActiveObjectByName(string name)
        {
            Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>();
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].hideFlags == HideFlags.None)
                {
                    if (objs[i].name == name)
                    {
                        return objs[i].gameObject;
                    }
                }
            }
            return null;
        }
    }
}
