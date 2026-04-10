using HarmonyLib;
using NGame2.NUI.NWindow.NCutScene;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace KRQOL
{
    [HarmonyPatch(typeof(CutSceneMenu), "ShowSkipButton")]
    [HarmonyPatch(typeof(CutSceneMenu), "OpenWindow")]
    static class PatchSkipCutscene
    {
        static void Postfix(CutSceneMenu __instance, MethodBase __originalMethod)
        {
            if (!Settings.AutoSkipCutscene.Value) return;
            Plugin.DebugLog($"Auto skipping cutscene ({__originalMethod.Name}).");
            __instance.StartCoroutine(WaitAndSkip(__instance));
        }

        static IEnumerator WaitAndSkip(CutSceneMenu __instance)
        {
            Plugin.DebugLog("Auto skipping cutscene (ShowSkipButton)");
            yield return new WaitForSeconds(1.0f);
            __instance.OnClickSkip();
        }
    }
}
