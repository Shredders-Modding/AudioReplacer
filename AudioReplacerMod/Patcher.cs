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
                ModLogger.Log($"{__instance.gameObject.name} rider show up. Replacing its sounds");
                ModManager.ReplaceGameplayRiderSounds(__instance.gameObject);
                /*
                if (__instance == ModManager.userSession.sc && !ModManager.areGameplayRidersRegistered)
                {
                    ModManager.gameplayRider.Add(__instance.gameObject);
                    
                    for (int i = 0; i < 32; i++)
                    {
                        GameObject rider = ModManager.FindInActiveObjectByName("Snowboarder2_" + i);
                    }

                    ModLogger.Log($"{ModManager.gameplayRider.Count} gameplayRiders found");
                    ModManager.gameplaySnowboardSounds = __instance.gameObject.GetComponent<Lirp.SnowboardSounds>();
                    ModLogger.Log("SnowboardSounds found");
                    ModManager.areGameplayRidersRegistered = true;
                    ModManager.ReplaceGameplayRidersSounds();
                }
                */
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
