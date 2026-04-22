using JCC.LifeCycle;
using System.Collections;
using UnityEngine;

namespace JCC.Utils.Art
{
    public class SpriteFadeProcessor : MonoBehaviour, ILifeCycle
    {
        private bool _wasInitialized = false;
        private float _currentFadeDuration = 0;
        private Coroutine _currentCoroutine = null;
        private IColorChanger _colorChanger = null;
        private float _fadeEnd = 0;
        private float _fadeInit = 0;

        public bool WasInitialized() => _wasInitialized;
        public bool IsProcessFade() => _currentCoroutine != null;

        public bool Initialization(params object[] parameters)
        {
            _colorChanger = parameters[0] as IColorChanger;
            _wasInitialized = _colorChanger != null ? true : false;
            return _wasInitialized;
        }

        public void SetAlpha(float newAlpha) 
        {
            Color originalColor = _colorChanger.GetColor();
            originalColor.a = newAlpha;
            _colorChanger.SetColor(originalColor);
        }

        public void StartFade(float fadeDuration, float fadeInit, float fadeEnd) 
        {
            _fadeEnd = fadeEnd;
            _fadeInit = fadeInit;
            _currentFadeDuration = fadeDuration;
            StopCurrentCoroutine();
            _currentCoroutine = StartCoroutine(StartFadeCoroutine(true));
        }

        public void StartPingPong(float fadeDuration, float fadeInit, float fadeEnd)
        {
            _fadeEnd = fadeEnd;
            _fadeInit = fadeInit;
            _currentFadeDuration = fadeDuration/2;
            StopCurrentCoroutine();
            _currentCoroutine = StartCoroutine(PingPong());
        }

        private void StopCurrentCoroutine() 
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }
        }

        private IEnumerator StartFadeCoroutine(bool stopCoroutine)
        {
            float timeElapsed = 0f;
            while (timeElapsed < _currentFadeDuration)
            {
                float alpha = Mathf.Lerp(_fadeInit, _fadeEnd, timeElapsed / _currentFadeDuration);
                SetAlpha(alpha);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            SetAlpha(_fadeEnd);
            if (stopCoroutine)
            {
                StopCurrentCoroutine();
            }
        }

        private IEnumerator PingPong()
        {
            yield return StartFadeCoroutine(false);
            _fadeInit = _fadeEnd;
            _fadeEnd = 0f;
            yield return StartFadeCoroutine(false);
            StopCurrentCoroutine();
        }
    }
}
