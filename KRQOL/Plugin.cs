using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;

namespace KRQOL
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        internal static RoutineClearCampaign _routineClearCampaign;
        internal static string lastLog = "";


        private static PluginWindow _window;
        private static readonly List<Routine> _routines = new List<Routine>();


        private void Awake()
        {
            Logger = base.Logger;

            Settings.Setup(Config);

            _routineClearCampaign = new RoutineClearCampaign(this);
            _routines.Add(_routineClearCampaign);

            _window = new PluginWindow(_routines, Settings.ShowWindowAtStart.Value);

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} is loaded!");

            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
            foreach (var method in harmony.GetPatchedMethods())
            {
                Plugin.Logger.LogInfo($"Patched: {method.DeclaringType?.Name}.{method.Name}");
            }
        }

        private void Update()
        {
            foreach (var routine in _routines)
                routine.Update();

            _window.Update();
        }

        private void OnGUI()
        {
            _window.OnGUI();
        }

        internal static void DebugLog(object data)
        {
            if (!Settings.DebugLogging.Value) return;
            Logger.LogInfo(data);
            lastLog = data.ToString();
        }
    }
}
