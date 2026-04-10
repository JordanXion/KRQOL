using HarmonyLib;
using NGame2.NUI.NWindow.NBattle;
using NVespa.NSingleton;
using System.Collections;
using UnityEngine;

namespace KRQOL
{
    [HarmonyPatch(typeof(EndBattleReward), "Init")]
    static class PatchAutoNextBattle
    {
        static void Postfix(EndBattleReward __instance)
        {
            if (!Settings.AutoNextBattle.Value) return;

            Plugin.DebugLog("EndBattleReward opened - waiting for buttons");

            if (AutoRepeatBattle.IsEnabled() || AutoNextBattle.IsEnabled())
            {
                Plugin.DebugLog($"In-game continuous battle is enabled, skipping auto next battle. AutoRepeatBattle={AutoRepeatBattle.IsEnabled()} AutoNextBattle={AutoNextBattle.IsEnabled()}");
                return;
            }

            Plugin.DebugLog("Attempting to press next dungeon button");
            __instance.StartCoroutine(WaitAndNextDungeon(__instance));
        }

        static IEnumerator WaitAndNextDungeon(EndBattleReward reward)
        {
            if (reward == null)
            {
                Plugin.DebugLog("Reward is null, skipping.");
                yield break;
            }
            yield return new WaitForSeconds(reward.ButtonDelay + 0.1f);

            if (reward.Button_NextDungeon == null || !reward.Button_NextDungeon.isEnabled)
            {
                Plugin.DebugLog("Next dungeon button not available, skipping.");
                yield break;
            }

            Plugin.DebugLog("Auto clicking next dungeon.");
            yield return new WaitForSeconds(Settings.AutoNextBattleDelay.Value);
            reward.OnClickNextDungeon();
        }
    }
}
