using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

namespace AudioReplacerMod
{
    [HarmonyPatch(typeof(Lirp.SnowboardController), "Show")]
    class SnowboardControllerPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(System.Reflection.MethodBase __originalMethod, Lirp.SnowboardController __instance)
        {
            try
            {
                if (__instance == ModManager.userSession.sc && !ModManager.gameplayRider)
                {
                    ModManager.gameplayRider = __instance.gameObject;
                    ModLogger.Log("GameplayRider found");
                    ModManager.gameplaySnowboardSounds = __instance.gameObject.GetComponent<Lirp.SnowboardSounds>();
                    ModLogger.Log("SnowboardSounds found");
                }
            }
            catch (System.Exception ex)
            {
                //MelonLogger.Msg($"Exception in patch of SnowboardSounds.OnLand:\n{ex}");
            }
        }
    }

    [HarmonyPatch(typeof(PlayerRigReplayManager), "SetupReplays")]
    class ReplayManagerShowPlayerPatch
    {
        [HarmonyPostfix]
        public static void Postfix(System.Reflection.MethodBase __originalMethod, PlayerRigReplayManager __instance)
        {
            try
            {
                ModLogger.Log("Replay started");
                if (!ModManager.replayRider)
                {
                    ModManager.replayRider = PlayerRigReplayManager._instance._instantiatedRider.scGO;
                    ModLogger.Log("ReplayRider found");
                    ModManager.replaySnowboardSounds = ModManager.replayRider.GetComponent<Lirp.SnowboardSounds>();
                    ModLogger.Log("SnowboardSounds found");
                }
            }
            catch (System.Exception ex)
            {
                //MelonLogger.Msg($"Exception in patch of SnowboardSounds.OnLand:\n{ex}");
            }
        }
    }
}
