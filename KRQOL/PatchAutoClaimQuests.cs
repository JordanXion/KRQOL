using HarmonyLib;
using NGame2.NUI.NWindow;

namespace KRQOL
{
    [HarmonyPatch(typeof(QuestList), "RefreshList")]
    static class PatchAutoClaimQuests
    {
        static void Postfix(QuestList __instance)
        {
            if (!Settings.AutoClaimRewards.Value) return;

            var selectedMainCategory = (int)AccessTools.Property(typeof(QuestList), "SelectedMainCategory").GetValue(__instance);
            Plugin.DebugLog($"QuestList refreshed - SelectedMainCategory = {selectedMainCategory}");
            if (selectedMainCategory == 9)
            {
                Plugin.DebugLog("Etc. category selected, skipping auto claim to avoid potential infinite loop.");
                return;
            }

            if (__instance.GetAllRewardButton != null && __instance.GetAllRewardButton.isEnabled)
            {
                Plugin.DebugLog("Auto clicking claim all.");
                __instance.OnClickAllGet();
            }

            if (__instance.QuestRewardUI != null &&
                __instance.QuestRewardUI.Button_Reward != null &&
                __instance.QuestRewardUI.Button_Reward.isEnabled)
            {
                Plugin.DebugLog("Auto clicking claim reward.");
                __instance.OnClickReward();
            }
        }
    }
}
