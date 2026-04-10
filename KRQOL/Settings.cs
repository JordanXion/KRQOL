using BepInEx.Configuration;

namespace KRQOL
{
    internal static class Settings
    {
        internal static ConfigEntry<bool> AutoLogin;
        internal static ConfigEntry<bool> AutoSkipCutscene;
        internal static ConfigEntry<bool> AutoSkipVictoryScreen;
        internal static ConfigEntry<bool> AutoSkipRewards;
        internal static ConfigEntry<bool> AutoNextBattle;
        internal static ConfigEntry<float> AutoNextBattleDelay;
        internal static ConfigEntry<bool> AutoClaimRewards;
        internal static ConfigEntry<bool> AutoReceiveMail;
        internal static ConfigEntry<bool> ShowWindowAtStart;
        internal static ConfigEntry<bool> DebugLogging;

        internal static void Setup(ConfigFile config)
        {
            AutoLogin = config.Bind("General", "AutoLogin", true, "Automatically skips the tap to login screen");
            AutoSkipCutscene = config.Bind("General", "AutoSkipCutscene", true, "Automatically skip cutscenes");
            AutoSkipVictoryScreen = config.Bind("General", "AutoSkipVictoryScreen", true, "Automatically skips end of battle victory screens");
            AutoSkipRewards = config.Bind("General", "AutoSkipRewards", true, "Automatically closes reward popups");
            AutoNextBattle = config.Bind("General", "AutoNextBattle", true, "Automatically clicks the next battle button on end of battle screens");
            AutoNextBattleDelay = config.Bind("General", "AutoNextBattleDelay", 5.0f, new ConfigDescription("How long to wait before clicking next battle (seconds). Setting this too low may cause issues due to the rewards screen.", new AcceptableValueRange<float>(0f, 60f)));
            AutoClaimRewards = config.Bind("General", "AutoClaimRewards", true, "Automatically clicks claim all on quest/achievement screens. Excludes \"Etc.\" quest category.");
            AutoReceiveMail = config.Bind("General", "AutoReceiveMail", true, "Automatically clicks receive all on the mail screen");
            ShowWindowAtStart = config.Bind("General", "ShowWindowAtStart", true, "Show the mod window on game start");
            DebugLogging = config.Bind("General", "DebugLogging", false, "Log verbose debug information");
        }
    }
}
