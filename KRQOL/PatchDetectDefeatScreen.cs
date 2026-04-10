using HarmonyLib;
using NGame2.NUI.NWindow.NBattle;

namespace KRQOL
{
    [HarmonyPatch(typeof(EndBattleLose), "Init")]
    static class PatchDetectDefeatScreen
    {
        static void Postfix()
        {
            if (!Plugin._routineClearCampaign.IsRunning) return;

            Plugin.DebugLog("Defeat screen detected while CampaignClear routine is running - stopping routine.");
            Plugin._routineClearCampaign.Stop();
        }
    }
}
