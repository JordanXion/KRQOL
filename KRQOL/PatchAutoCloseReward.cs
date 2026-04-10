using HarmonyLib;
using NGame2.NUI.NWindow.NBattle;
using System.Collections;
using UnityEngine;

namespace KRQOL
{
    [HarmonyPatch(typeof(BaseRewardPopup), "OpenWithData")]
    static class PatchAutoCloseReward
    {
        static void Postfix(BaseRewardPopup __instance, bool __result)
        {
            if (!Settings.AutoSkipRewards.Value) return;

            Plugin.DebugLog($"Reward popup detected - attempting to close");
            Plugin.DebugLog($"Popup open result = {__result}");

            if (!__result) return;
            __instance.StartCoroutine(WaitAndClose(__instance));
        }

        static IEnumerator WaitAndClose(BaseRewardPopup popup)
        {
            Plugin.DebugLog($"Attempting to close reward popup after input disabled time ({popup.InputDisabledTime}s)");
            yield return new WaitForSeconds(popup.InputDisabledTime + 0.1f);
            if (popup == null) yield break;
            popup.OnClickBackground();

            // animations may still be playing, call again after SkippableTime
            Plugin.DebugLog($"Attempting to close reward popup after skippable time ({popup.SkippableTime}s)");
            yield return new WaitForSeconds(popup.SkippableTime + 0.1f);
            if (popup == null) yield break;
            popup.OnClickBackground();
        }
    }
}
