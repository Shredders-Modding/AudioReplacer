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

        public static GameObject gameplayRider;
        public static GameObject replayRider;
        public static SnowboardSounds gameplaySnowboardSounds;
        public static SnowboardSounds replaySnowboardSounds;

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

        /*
        public override void OnLateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
                snowboardSounds.courseClips.Forward = null;
        }
        */
    }
}
