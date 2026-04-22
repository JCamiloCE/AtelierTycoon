using JCC.Debug;
using JCC.Fade;
using JCC.Music;
using JCC.Utils.Singleton;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace JCC.Scenes
{
    public class SceneController<TEnum> : SingletonMonoBehaviour<SceneController<TEnum>> where TEnum : Enum
    {
        private Dictionary<TEnum, string> _scenesNamesByIds = null;

        #region Singleton
        protected override void AfterSingletonCreation()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            FadeController.Instance.ForceFadeIn();
        }
        #endregion Singleton

        #region public
        public void SetScenes(Dictionary<TEnum, string> scenes) 
        {
            _scenesNamesByIds = new Dictionary<TEnum, string>();
            _scenesNamesByIds = scenes;
        }

        public void LoadScene(TEnum sceneId) 
        {
            string name = string.Empty;
            _scenesNamesByIds.TryGetValue(sceneId, out name);

            if (string.IsNullOrEmpty(name)) 
            {
                DebugManager.LogError($"Try to load {sceneId} but that scene doesn't exist");
                return;
            }

            Action cb = () => { SceneManager.LoadScene(name); };
            float fadeTime = 1f;
            FadeController.Instance.FadeIn(fadeTime, true, true, cb);
            AudioController.Instance.ExitScene(fadeTime/1.5f);
        }
        #endregion public

        #region private 
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            float fadeTime = 1f;
            FadeController.Instance.FadeOut(fadeTime, true, false, null, 0.1f);
            AudioController.Instance.EnterScene(fadeTime*2);
        }
        #endregion private
    }
}