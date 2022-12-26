using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using Il2CppLirp;

namespace AudioReplacerMod
{
    [HarmonyPatch(typeof(SnowboardController), "Show")]
    internal class SnowboardControllerPatcher
    {
        [HarmonyPostfix]
        public static void Postfix(System.Reflection.MethodBase __originalMethod, SnowboardController __instance)
        {
            try
            {
                ModLogger.Log($"{__instance.gameObject.name} rider show up. Replacing its sounds");
                ModManager.ReplaceRiderSounds(__instance.gameObject);
            }
            catch (System.Exception ex)
            {
                //MelonLogger.Msg($"Exception in patch of SnowboardSounds.OnLand:\n{ex}");
            }
        }
    }
}
