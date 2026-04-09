using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using NGame2.NCutScene;
using NGame2.NUI.NWindow;
using NGame2.NUI.NWindow.Mission;
using NGame2.NUI.NWindow.NBattle;
using NGame2.NUI.NWindow.NCutScene;
using NVespa.NSingleton;
using System;
using System.Collections;
using UnityEngine;

namespace KRQOL
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        internal static ConfigEntry<bool> AutoLogin;
        internal static ConfigEntry<bool> AutoSkipCutscene;
        internal static ConfigEntry<float> CutsceneBailoutTime;
        internal static ConfigEntry<bool> AutoSkipVictoryScreen;
        internal static ConfigEntry<bool> AutoSkipRewards;
        internal static ConfigEntry<bool> AutoNextBattle;
        internal static ConfigEntry<bool> AutoClaimRewards;
        internal static ConfigEntry<bool> AutoReceiveMail;
        internal static ConfigEntry<bool> DebugLogging;


        private void Awake()
        {
            Logger = base.Logger;

            AutoLogin = Config.Bind("General", "AutoLogin", true, "Automatically skips the tap to login screen");
            AutoSkipCutscene = Config.Bind("General", "AutoSkipCutscene", true, "Automatically skip cutscenes");
            CutsceneBailoutTime = Config.Bind("General", "CutsceneBailoutTime", 10f, new ConfigDescription("How long to wait before giving up on skipping a cutscene (seconds)", new AcceptableValueRange<float>(1f, 60f)));
            AutoSkipVictoryScreen = Config.Bind("General", "AutoSkipVictoryScreen", true, "Automatically skips end of battle victory screens");
            AutoSkipRewards = Config.Bind("General", "AutoSkipRewards", true, "Automatically closes reward popups");
            AutoNextBattle = Config.Bind("General", "AutoNextBattle", true, "Automatically clicks the next battle button on end of battle screens");
            AutoClaimRewards = Config.Bind("General", "AutoClaimRewards", true, "Automatically clicks claim all on quest/achievement screens. Excludes \"Etc.\" quest category.");
            AutoReceiveMail = Config.Bind("General", "AutoReceiveMail", true, "Automatically clicks receive all on the mail screen");
            DebugLogging = Config.Bind("General", "DebugLogging", false, "Log verbose debug information");

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} is loaded!");

            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
            foreach (var method in harmony.GetPatchedMethods())
            {
                Plugin.Logger.LogInfo($"Patched: {method.DeclaringType?.Name}.{method.Name}");
            }
        }

        internal static void DebugLog(object data)
        {
            if (!DebugLogging.Value) return;
            Logger.LogInfo(data);
        }
    }


    [HarmonyPatch(typeof(LoginTouchWait), "Init")]
    static class PatchSkipLoginScreen
    {
        static void Postfix(Action onTouch)
        {
            if (!Plugin.AutoLogin.Value) return;

            Plugin.DebugLog($"Login screen detected - attempting to login");
            onTouch?.Invoke();
        }
    }


    [HarmonyPatch(typeof(CutSceneMenu), "ShowSkipButton")]
    static class PatchSkipCutscene
    {
        static bool _isRunningSkip = false;
        static void Postfix(CutSceneMenu __instance, bool isShow)
        {
            if (!Plugin.AutoSkipCutscene.Value) return;

            Plugin.DebugLog($"Cutscene detected - attempting to skip");
            Plugin.DebugLog($"Cutscene isShow = {isShow}");
            Plugin.DebugLog($"CutsceneManager valid = {SceneAutoBehaviour<CutSceneManager>.isValidInstance}");
            Plugin.DebugLog($"CutsceneManager isPlaying = {SceneAutoBehaviour<CutSceneManager>.isValidInstance && SceneAutoBehaviour<CutSceneManager>.instance.IsPlaying}");
            //if (!isShow) return;

            if (_isRunningSkip) return;
            __instance.StartCoroutine(WaitAndSkip());
        }

        static IEnumerator WaitAndSkip()
        {
            _isRunningSkip = true;
            float elapsed = 0f;
            while (!SceneAutoBehaviour<CutSceneManager>.isValidInstance ||
                   !SceneAutoBehaviour<CutSceneManager>.instance.IsPlaying)
            {
                Plugin.DebugLog($"Waiting... elapsed={elapsed:F1}s valid={SceneAutoBehaviour<CutSceneManager>.isValidInstance} playing={SceneAutoBehaviour<CutSceneManager>.isValidInstance && SceneAutoBehaviour<CutSceneManager>.instance.IsPlaying}");
                elapsed += 0.1f;
                if (elapsed >= Plugin.CutsceneBailoutTime.Value)
                {
                    Plugin.DebugLog("Cutscene skip timed out.");
                    _isRunningSkip = false;
                    yield break;
                }
                yield return new WaitForSeconds(0.1f);
            }
            Plugin.DebugLog("Cutscene skipped.");
            SceneAutoBehaviour<CutSceneManager>.instance.Skip();
            _isRunningSkip = false;
        }
    }


    [HarmonyPatch(typeof(EndBattleWin), "Init")]
    static class PatchSkipVictoryScreen
    {
        static void Postfix(EndBattleWin __instance)
        {
            if (!Plugin.AutoSkipVictoryScreen.Value) return;

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


    [HarmonyPatch(typeof(BaseRewardPopup), "OpenWithData")]
    static class PatchAutoCloseReward
    {
        static void Postfix(BaseRewardPopup __instance, bool __result)
        {
            if (!Plugin.AutoSkipRewards.Value) return;

            Plugin.DebugLog($"Reward popup detected - attempting to close");
            Plugin.DebugLog($"Popup open result = {__result}");

            if (!__result) return;
            __instance.StartCoroutine(WaitAndClose(__instance));
        }

        static IEnumerator WaitAndClose(BaseRewardPopup popup)
        {
            Plugin.DebugLog($"Attempting to close reward popup after input disabled time ({popup.InputDisabledTime}s)");
            yield return new WaitForSeconds(popup.InputDisabledTime + 0.1f);
            Plugin.DebugLog($"Popup valid check 1 = {popup != null}");
            if (popup == null) yield break;
            popup.OnClickBackground();

            // animations may still be playing, call again after SkippableTime
            Plugin.DebugLog($"Attempting to close reward popup after skippable time ({popup.SkippableTime}s)");
            yield return new WaitForSeconds(popup.SkippableTime + 0.1f);
            Plugin.DebugLog($"Popup valid check 2 = {popup != null}");
            if (popup == null) yield break;
            popup.OnClickBackground();
        }
    }


    [HarmonyPatch(typeof(EndBattleReward), "Init")]
    static class PatchAutoNextBattle
    {
        static void Postfix(EndBattleReward __instance)
        {
            if (!Plugin.AutoNextBattle.Value) return;

            if (AutoRepeatBattle.IsEnabled() || AutoNextBattle.IsEnabled())
            {
                Plugin.DebugLog($"In-game continuous battle is enabled, skipping auto next battle. AutoRepeatBattle={AutoRepeatBattle.IsEnabled()} AutoNextBattle={AutoNextBattle.IsEnabled()}");
                return;
            }

            Plugin.DebugLog("EndBattleReward opened - waiting for buttons");
            __instance.StartCoroutine(WaitAndNextDungeon(__instance));
        }

        static IEnumerator WaitAndNextDungeon(EndBattleReward reward)
        {
            yield return new WaitForSeconds(reward.ButtonDelay + 0.1f);
            if (reward == null) yield break;

            if (reward.Button_NextDungeon == null || !reward.Button_NextDungeon.isEnabled)
            {
                Plugin.DebugLog("Next dungeon button not available, skipping.");
                yield break;
            }

            Plugin.DebugLog("Auto clicking next dungeon.");
            reward.OnClickNextDungeon();
        }
    }


    [HarmonyPatch(typeof(QuestList), "RefreshList")]
    static class PatchAutoClaimQuests
    {
        static void Postfix(QuestList __instance)
        {
            if (!Plugin.AutoClaimRewards.Value) return;

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

    [HarmonyPatch(typeof(HeroAchievementList), "SetupHeroAchievementList")]
    static class PatchAutoClaimHeroAchievements
    {
        static void Postfix(HeroAchievementList __instance)
        {
            if (!Plugin.AutoClaimRewards.Value) return;

            Plugin.DebugLog($"HeroAchievementList refreshed - checking for claimable rewards");
            Plugin.DebugLog($"Button_AllGet null = {__instance.Button_AllGet == null}");

            if (__instance.Button_AllGet == null) return;

            Plugin.DebugLog($"Button_AllGet enabled = {__instance.Button_AllGet.isEnabled}");

            if (!__instance.Button_AllGet.isEnabled) return;

            Plugin.DebugLog("Auto claiming hero achievement rewards.");
            __instance.OnClickGetAllReward();
        }
    }


    [HarmonyPatch(typeof(MissionNodeWindowTopView2), "SetGetAllRewardButton")]
    static class PatchAutoClaimMissionNodeRewards
    {
        static void Postfix(MissionNodeWindowTopView2 __instance, bool allGet)
        {
            if (!Plugin.AutoClaimRewards.Value) return;

            Plugin.DebugLog($"MissionNodeWindowTopView2 SetGetAllRewardButton called");
            Plugin.DebugLog($"allGet = {allGet}");

            if (!allGet) return;

            Plugin.DebugLog("Auto claiming mission node rewards.");
            __instance.OnClickGetAllReward();
        }
    }

    [HarmonyPatch(typeof(MailListPopup), "UpdateButtonState")]
    static class PatchAutoReceiveMail
    {
        static void Postfix(MailListPopup __instance)
        {
            if (!Plugin.AutoReceiveMail.Value) return;

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
