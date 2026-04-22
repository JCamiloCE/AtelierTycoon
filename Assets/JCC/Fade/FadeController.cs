using JCC.Utils.Singleton;
using System;
using System.Collections;
using UnityEngine;

namespace JCC.Fade 
{
    public class FadeController : SingletonMonoBehaviour<FadeController>
    {
        [SerializeField] private CanvasGroup _generalFade;

        private Coroutine _fade = null;

        #region Singleton
        protected override void AfterSingletonCreation()
        {
            _generalFade.alpha = 0f;
            BlockRaycast(false);
        }
        #endregion Singleton

        #region public
        public void FadeIn(float fadeTime, bool blockInit, bool blockEnd, Action callback, float delay = 0f) 
        {
            StopFadeCoroutine();
            _fade = StartCoroutine(StartFade(0f, 1f, fadeTime, blockInit, blockEnd, callback, delay));
        }

        public void FadeOut(float fadeTime, bool blockInit, bool blockEnd, Action callback, float delay = 0f) 
        {
            StopFadeCoroutine();
            _fade = StartCoroutine(StartFade(1f, 0f, fadeTime, blockInit, blockEnd, callback, delay));
        }

        public void ForceFadeIn() 
        {
            _generalFade.alpha = 1f;
            BlockRaycast(true);
        }
        #endregion public

        #region private
        private void StopFadeCoroutine() 
        {
            if (_fade != null) 
            {
                StopCoroutine(_fade);
                _fade = null;
            }
        }

        private IEnumerator StartFade(float initValue, float endValue, float fadeTime, bool blockInit, bool blockEnd, Action callback, float delay) 
        {
            yield return new WaitForSeconds(delay);
            BlockRaycast(blockInit);
            _generalFade.alpha = initValue;
            float currentTime = 0f;
            while (currentTime < fadeTime)
            {
                float newFadeValue = Mathf.Lerp(initValue, endValue, currentTime/fadeTime);
                _generalFade.alpha = newFadeValue;
                currentTime += Time.deltaTime;
                yield return null;
            }
            _generalFade.alpha = endValue;
            BlockRaycast(blockEnd);
            callback?.Invoke();
            _fade = null;
        }

        private void BlockRaycast(bool newState) 
        {
            _generalFade.blocksRaycasts = newState;
            _generalFade.interactable = newState;
        }
        #endregion private
    }
}