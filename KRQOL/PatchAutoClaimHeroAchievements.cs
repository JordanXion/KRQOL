using HarmonyLib;
using NGame2.NUI.NWindow;

namespace KRQOL
{
    [HarmonyPatch(typeof(HeroAchievementList), "SetupHeroAchievementList")]
    static class PatchAutoClaimHeroAchievements
    {
        static void Postfix(HeroAchievementList __instance)
        {
            if (!Settings.AutoClaimRewards.Value) return;

            Plugin.DebugLog($"HeroAchievementList refreshed - checking for claimable rewards");
            Plugin.DebugLog($"Button_AllGet null = {__instance.Button_AllGet == null}");

            if (__instance.Button_AllGet == null) return;

            Plugin.DebugLog($"Button_AllGet enabled = {__instance.Button_AllGet.isEnabled}");

            if (!__instance.Button_AllGet.isEnabled) return;

            Plugin.DebugLog("Auto claiming hero achievement rewards.");
            __instance.OnClickGetAllReward();
        }
    }
}
