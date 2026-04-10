using System.Collections.Generic;
using UnityEngine;

namespace KRQOL
{
    internal class PluginWindow
    {
        private readonly List<Routine> _routines;

        private bool _showGui = true;
        private Rect _windowRect;
        private string _nextBattleDelayInput;

        internal PluginWindow(List<Routine> routines, bool showAtStart)
        {
            _routines = routines;
            _showGui = showAtStart;

            float width = 300;
            float height = 500;
            float x = Screen.width * 0.1f;
            float y = (Screen.height - height) / 2f;
            _windowRect = new Rect(x, y, width, height);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Home)) _showGui = !_showGui;
        }

        public void OnGUI()
        {
            if (!_showGui) return;
            _windowRect = GUI.Window(0, _windowRect, DrawWindow, "KRQOL");
        }

        private void DrawWindow(int id)
        {
            GUILayout.BeginVertical();

            DrawSettingsSection();

            GUILayout.Space(10);

            DrawRoutinesSection();

            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private void DrawSettingsSection()
        {
            GUILayout.Label("HOME to toggle this window");
            GUILayout.Label("--- Settings ---");
            Settings.AutoLogin.Value = GUILayout.Toggle(Settings.AutoLogin.Value, "Auto Login");
            Settings.AutoSkipCutscene.Value = GUILayout.Toggle(Settings.AutoSkipCutscene.Value, "Auto Skip Cutscene");
            Settings.AutoSkipVictoryScreen.Value = GUILayout.Toggle(Settings.AutoSkipVictoryScreen.Value, "Auto Skip Victory Screen");
            Settings.AutoSkipRewards.Value = GUILayout.Toggle(Settings.AutoSkipRewards.Value, "Auto Skip Rewards");
            
            GUILayout.BeginHorizontal();
            Settings.AutoNextBattle.Value = GUILayout.Toggle(Settings.AutoNextBattle.Value, "Auto Next Battle");
            GUILayout.Label("  Delay (s):", GUILayout.Width(80));
            _nextBattleDelayInput = GUILayout.TextField(_nextBattleDelayInput ?? Settings.AutoNextBattleDelay.Value.ToString("F1"), GUILayout.Width(50));
            if (float.TryParse(_nextBattleDelayInput, out float delay))
                Settings.AutoNextBattleDelay.Value = delay;
            GUILayout.EndHorizontal();

            Settings.AutoClaimRewards.Value = GUILayout.Toggle(Settings.AutoClaimRewards.Value, "Auto Claim Quests/Etc");
            Settings.AutoReceiveMail.Value = GUILayout.Toggle(Settings.AutoReceiveMail.Value, "Auto Receive Mail");
            Settings.ShowWindowAtStart.Value = GUILayout.Toggle(Settings.ShowWindowAtStart.Value, "Show This Window at Start");
            Settings.DebugLogging.Value = GUILayout.Toggle(Settings.DebugLogging.Value, "Debug Logging");


        }

        private void DrawRoutinesSection()
        {
            GUILayout.Label("--- Routines ---");
            foreach (var routine in _routines)
            {
                GUILayout.BeginHorizontal();

                GUIStyle statusStyle = new GUIStyle(GUI.skin.label);
                statusStyle.normal.textColor = routine.IsRunning ? Color.green : Color.red;
                GUILayout.Label($"{routine.Name}: {(routine.IsRunning ? "RUNNING" : "STOPPED")}", statusStyle);

                GUI.enabled = !routine.IsRunning;
                if (GUILayout.Button("Start", GUILayout.Width(50)))
                    routine.IsRunning = true;

                GUI.enabled = routine.IsRunning;
                if (GUILayout.Button("Stop", GUILayout.Width(50)))
                    routine.IsRunning = false;

                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
            
        }


    }
}
