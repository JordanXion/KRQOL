using HarmonyLib;
using NGame2.NUI.NWindow.NBattle;
using System.Collections;
using UnityEngine;

namespace KRQOL
{
    [HarmonyPatch(typeof(EndBattleWin), "Init")]
    static class PatchSkipVictoryScreen
    {
        static void Postfix(EndBattleWin __instance)
        {
            if (!Settings.AutoSkipVictoryScreen.Value) return;

            Plugin.DebugLog($"Victory screen detected - attempting to skip");
            __instance.StartCoroutine(WaitAndTouch(__instance));
        }

        static IEnumerator WaitAndTouch(EndBattleWin win)
        {
            float delay = win.InputDisabledTimeMs / 1000f + 0.1f;
            Plugin.DebugLog($"Waiting for input to be enabled ({delay}s)");
            yield return new WaitForSeconds(delay);
            Plugin.DebugLog($"Win screen valid = {win != null}");
            if (win == null) yield break;
            win.OnTouchScreen();
            Plugin.DebugLog("Victory screen skipped.");
        }
    }
}
