using System;
using UnityEngine;

namespace KRQOL
{
    internal abstract class Routine
    {
        protected readonly Plugin _plugin;

        private bool _isRunning;
        internal bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (value == _isRunning) return;
                if (value) Start();
                else Stop();
            }
        }

        internal virtual string Name => GetType().Name;

        internal Routine(Plugin plugin)
        {
            _plugin = plugin;
        }

        internal virtual void Start()
        {
            Plugin.DebugLog($"Starting routine: {Name}");
            _isRunning = true;
        }

        internal virtual void Stop()
        {
            Plugin.DebugLog($"Stopping routine: {Name}");
            _isRunning = false;
        }

        internal virtual void Update() { }

        internal bool OnFailure()
        {
            IsRunning = false;
            return false;
        }

        internal void Log(object data)
        {
            Plugin.DebugLog($"[{Name}] {data}");
        }

        internal bool FindAndClick<T>(Action<T> clickAction) where T : MonoBehaviour
        {
            var obj = GameObject.FindAnyObjectByType<T>();
            if (obj == null)
            {
                Log($"{typeof(T).Name} not found.");
                return OnFailure();
            }
            if (!obj.isActiveAndEnabled)
            {
                Log($"{typeof(T).Name} is not active/enabled.");
                return OnFailure();
            }

            Log($"Clicking {typeof(T).Name}.");
            clickAction(obj);
            return true;
        }
    }
}
