using Emc2.Scripts.Enums;
using JCC.Debug;
using JCC.Fade;
using JCC.Music;
using JCC.Utils.Singleton;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Emc2.Scripts.Scenes
{
    public class SceneController : SingletonMonoBehaviour<SceneController>
    {
        private Dictionary<ESceneIds, string> _scenesNamesByIds = null;

        #region Singleton
        protected override void AfterSingletonCreation()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            FadeController.Instance.ForceFadeIn();
            _scenesNamesByIds = new Dictionary<ESceneIds, string>();
            _scenesNamesByIds.Add(ESceneIds.MainMenu, "MainMenu");
            _scenesNamesByIds.Add(ESceneIds.Gameplay, "Gameplay");
            _scenesNamesByIds.Add(ESceneIds.Intermediate, "IntermediateInit");
        }
        #endregion Singleton

        #region public
        public void LoadScene(ESceneIds sceneId) 
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