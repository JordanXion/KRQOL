using HarmonyLib;
using NGame2.NUI.NWindow;

namespace KRQOL
{
    [HarmonyPatch(typeof(MailListPopup), "UpdateButtonState")]
    static class PatchAutoReceiveMail
    {
        static void Postfix(MailListPopup __instance)
        {
            if (!Settings.AutoReceiveMail.Value) return;

            Plugin.DebugLog($"MailListPopup UpdateButtonState called");
            Plugin.DebugLog($"ReceiveAllButton null = {__instance.ReceiveAllButton == null}");

            if (__instance.ReceiveAllButton == null) return;

            Plugin.DebugLog($"ReceiveAllButton enabled = {__instance.ReceiveAllButton.isEnabled}");

            if (!__instance.ReceiveAllButton.isEnabled) return;

            Plugin.DebugLog("Auto receiving all mail.");
            __instance.OnClickReceiveAllMail();
        }
    }
}
