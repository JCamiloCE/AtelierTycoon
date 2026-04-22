using UnityEngine;

namespace JCC.Utils.Singleton
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Singleton
        public static T Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
                AfterSingletonCreation();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected abstract void AfterSingletonCreation();
        #endregion
    }
}