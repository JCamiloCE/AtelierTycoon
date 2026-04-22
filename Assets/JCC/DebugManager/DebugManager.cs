namespace JCC.Debug
{
    public static class DebugManager
    {
        private static IDebug _debugImpl = null;
        private static EDebugScope _debugScope = EDebugScope.All;
        private static bool _wasInitialized = false;

        public static bool IsInitialized => _wasInitialized;

        public static void Initialization(IDebug debug, EDebugScope debugScope)
        {
            if (_wasInitialized) 
            {
                UnityEngine.Debug.LogError("DebugManager::Initialization -> Was already initialized");
                return;
            }

            if (debug == null) 
            {
                UnityEngine.Debug.LogError("DebugManager::Initialization -> IDebug was null");
                return;
            }

            _debugImpl = debug;
            _debugScope = debugScope;
            _wasInitialized = true;
        }

        public static void LogVerbose(string message) 
        {
            if (CheckInitialization() && _debugScope == EDebugScope.All) 
            {
                _debugImpl.LogVerbose(message);
            }
        }

        public static void LogWarning(string message)
        {
            if (CheckInitialization() && _debugScope < EDebugScope.WarningAndError)
            {
                _debugImpl.LogWarning(message);
            }
        }

        public static void LogError(string message)
        {
            if (CheckInitialization() && _debugScope < EDebugScope.Error) 
            { 
                _debugImpl.LogError(message);
            }
        }

        private static bool CheckInitialization() 
        {
            if (!_wasInitialized)
            {
                UnityEngine.Debug.LogError("DebugManager wasn't initialized");
            }
            return _wasInitialized;
        }

#if UNITY_EDITOR
        public static void Debug__ResetDebugManager() 
        {
            _debugImpl = null;
            _debugScope = EDebugScope.All;
            _wasInitialized = false;
        }   
#endif
    }
}