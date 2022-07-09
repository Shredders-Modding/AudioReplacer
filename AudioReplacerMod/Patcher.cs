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
                ModManager.ReplaceRiderSounds(__instance.gameObject);
            }
            catch (System.Exception ex)
            {
                //MelonLogger.Msg($"Exception in patch of SnowboardSounds.OnLand:\n{ex}");
            }
        }
    }
}
