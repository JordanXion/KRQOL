using HarmonyLib;
using NGame2.NUI.NWindow.Mission;

namespace KRQOL
{
    [HarmonyPatch(typeof(MissionNodeWindowTopView2), "SetGetAllRewardButton")]
    static class PatchAutoClaimMissionNodeRewards
    {
        static void Postfix(MissionNodeWindowTopView2 __instance, bool allGet)
        {
            if (!Settings.AutoClaimRewards.Value) return;

            Plugin.DebugLog($"MissionNodeWindowTopView2 SetGetAllRewardButton called");
            Plugin.DebugLog($"allGet = {allGet}");

            if (!allGet) return;

            Plugin.DebugLog("Auto claiming mission node rewards.");
            __instance.OnClickGetAllReward();
        }
    }
}
