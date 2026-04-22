using JCC.Utils.Singleton;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace JCC.Music
{
    public class AudioController : SingletonMonoBehaviour<AudioController>
    {
        private const string MASTER_VOL = "MasterVol";
        private const string MUSIC_VOL = "MusicVol";
        private const string EFFECTS_VOL = "EffectsVol";
        private const string IS_MUSIC_ACTIVE_KEY = "MusicActiveKey";
        private const string IS_EFFECTS_ACTIVE_KEY = "EffectsActiveKey";
        private const float NO_SOUND_VALUE = 0.0001f;

        [SerializeField] private AudioMixer _audioMixer;

        private Coroutine _fadeMasterVol = null;
        private Coroutine _fadeMusicVol = null;

        private bool _isMusicActive = true;
        private bool _isEffectsActive = true;

        #region Singleton
        protected override void AfterSingletonCreation()
        {
            Invoke(nameof(LoadVols), 0.2f);
        }
        #endregion Singleton

        #region public
        public bool GetIsMusicActive => _isMusicActive;
        public bool GetIsEffectsActive => _isEffectsActive;

        public void ExitScene(float fadeTime)
        {
            StopFadeCoroutine(_fadeMasterVol);
            _fadeMasterVol = StartCoroutine(StartFade(1f, NO_SOUND_VALUE, fadeTime, 0f, _audioMixer, MASTER_VOL, _fadeMasterVol));
        }

        public void EnterScene(float fadeTime)
        {
            StopFadeCoroutine(_fadeMasterVol);
            _fadeMasterVol = StartCoroutine(StartFade(NO_SOUND_VALUE, 1f, fadeTime, 0f, _audioMixer, MASTER_VOL, _fadeMasterVol));
        }

        public void MuteMusic(float fadeTime, bool isMute, float delay) 
        {
            StopFadeCoroutine(_fadeMusicVol);
            float initialVal = isMute ? 1f : NO_SOUND_VALUE;
            float endVal = isMute ? NO_SOUND_VALUE : 1f;
            _fadeMusicVol = StartCoroutine(StartFade(initialVal, endVal, fadeTime, delay, _audioMixer, MASTER_VOL, _fadeMasterVol));
        }

        public void SetActiveMusic() 
        {
            _isMusicActive = !_isMusicActive;
            float newVol = _isMusicActive ? 1f : NO_SOUND_VALUE;
            float dB = Mathf.Log10(newVol) * 20f;
            _audioMixer.SetFloat(MUSIC_VOL, dB);
            PlayerPrefs.SetInt(IS_MUSIC_ACTIVE_KEY, _isMusicActive ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetActiveEffects()
        {
            _isEffectsActive = !_isEffectsActive;
            float newVol = _isEffectsActive ? 1f : NO_SOUND_VALUE;
            float dB = Mathf.Log10(newVol) * 20f;
            _audioMixer.SetFloat(EFFECTS_VOL, dB);
            PlayerPrefs.SetInt(IS_EFFECTS_ACTIVE_KEY, _isEffectsActive ? 1 : 0);
            PlayerPrefs.Save();
        }
        #endregion public

        #region private
        private void LoadVols() 
        {
            if (PlayerPrefs.HasKey(IS_MUSIC_ACTIVE_KEY) && PlayerPrefs.GetInt(IS_MUSIC_ACTIVE_KEY) == 0)
            {
                _isMusicActive = false;
                float dB = Mathf.Log10(NO_SOUND_VALUE) * 20f;
                _audioMixer.SetFloat(MUSIC_VOL, dB);
            }

            if (PlayerPrefs.HasKey(IS_EFFECTS_ACTIVE_KEY) && PlayerPrefs.GetInt(IS_EFFECTS_ACTIVE_KEY) == 0)
            {
                _isEffectsActive = false;
                float dB = Mathf.Log10(NO_SOUND_VALUE) * 20f;
                _audioMixer.SetFloat(EFFECTS_VOL, dB);
            }
        }

        private void StopFadeCoroutine(Coroutine coroutine)
        {
            if (_fadeMasterVol != null)
            {
                StopCoroutine(_fadeMasterVol);
                _fadeMasterVol = null;
            }
        }

        private IEnumerator StartFade(float initValue, float endValue, float fadeTime, float delay, AudioMixer audioMixer, string exposedVariable, Coroutine coroutine)
        {
            yield return new WaitForSeconds(delay);
            float dB = Mathf.Log10(initValue) * 20f;
            audioMixer.SetFloat(exposedVariable, dB);
            float currentTime = 0f;
            while (currentTime < fadeTime)
            {
                float newFadeValue = Mathf.Lerp(initValue, endValue, currentTime / fadeTime);
                dB = Mathf.Log10(newFadeValue) * 20f;
                audioMixer.SetFloat(exposedVariable, dB);
                currentTime += Time.deltaTime;
                yield return null;
            }
            dB = Mathf.Log10(endValue) * 20f;
            audioMixer.SetFloat(exposedVariable, dB);
            coroutine = null;
        }
        #endregion private
    }
}