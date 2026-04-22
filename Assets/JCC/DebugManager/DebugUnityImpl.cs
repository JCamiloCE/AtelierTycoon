namespace JCC.Debug
{
    public class DebugUnityImpl : IDebug
    {
        void IDebug.LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        void IDebug.LogVerbose(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        void IDebug.LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }
    }
}