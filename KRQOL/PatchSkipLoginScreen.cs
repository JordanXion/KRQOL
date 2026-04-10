using HarmonyLib;
using NGame2.NUI.NWindow;
using System;

namespace KRQOL
{
    [HarmonyPatch(typeof(LoginTouchWait), "Init")]
    static class PatchSkipLoginScreen
    {
        static void Postfix(Action onTouch)
        {
            if (!Settings.AutoLogin.Value) return;

            Plugin.DebugLog($"Login screen detected - attempting to login");
            onTouch?.Invoke();
        }
    }
}
