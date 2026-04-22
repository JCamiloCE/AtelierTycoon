using JCC.Debug;
using JCC.Enums;
using JCC.GameplayEventSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JCC.Localization
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        private const string LANG_PREF_KEY = "UserLanguage";

        private List<string> _fileNames = null;
        private ILocalizationProcessor _processor;
        private ELanguageID _currentLanguage = ELanguageID.Invalid;

        public ELanguageID CurrentLanguage => _currentLanguage;

        #region public
        public void Initialize(ILocalizationProcessor processor, List<string> fileNames)
        {
            if (processor == null)
            {
                DebugManager.LogError("[LocalizationManager] Cannot initialized, processor is null");
                return;
            }
            if (fileNames == null || fileNames.Count == 0)
            {
                DebugManager.LogError("[LocalizationManager] Cannot initialized, fileNames is null or empty");
                return;
            }
            _processor = processor;
            _fileNames = fileNames;
            ChangeLanguage(GetInitialLanguage());
        }

        public void ChangeLanguage(ELanguageID langID)
        {
            if (langID == ELanguageID.Invalid)
            {
                DebugManager.LogError("[LocalizationManager] Cannot change language, langID is Invalid");
                return;
            }
            _currentLanguage = langID;
            PlayerPrefs.SetInt(LANG_PREF_KEY, (int)_currentLanguage);
            PlayerPrefs.Save();
            LoadAllFiles();
        }

        public string GetTranslation(string key)
        {
            if (_processor == null || !_processor.IsReady())
            {
                DebugManager.LogError("[LocalizationManager] Cannot get the translation, processor is not ready");
                return null;
            }

            if (string.IsNullOrEmpty(key))
            {
                DebugManager.LogError("[LocalizationManager] Cannot get the translation, key is null or empty");
                return key;
            }
            return _processor.GetValue(key);
        }
        #endregion public

        #region private
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private ELanguageID GetInitialLanguage() 
        {
            ELanguageID languageToSet = ELanguageID.Invalid;

            if (PlayerPrefs.HasKey(LANG_PREF_KEY))
            {
                int savedLang = PlayerPrefs.GetInt(LANG_PREF_KEY, (int)ELanguageID.Invalid);
                if (savedLang != (int)ELanguageID.Invalid)
                {
                    languageToSet = (ELanguageID)savedLang;
                }
            }

            if (languageToSet == ELanguageID.Invalid)
            {
                languageToSet = GetSystemLanguage();
            }
            return languageToSet;
        }

        private ELanguageID GetSystemLanguage()
        {
            ELanguageID fallback = ELanguageID.en;
            SystemLanguage systemLang = Application.systemLanguage;
            return systemLang switch
            {
                SystemLanguage.English => ELanguageID.en,
                SystemLanguage.Spanish => ELanguageID.sp,
                _ => fallback
            };
        }

        private void LoadAllFiles()
        {
            if (_currentLanguage == ELanguageID.Invalid)
            {
                DebugManager.LogError("[LocalizationManager] Cannot load files because _currentLanguage wasn't be set");
                return;
            }

            _processor.Clear();
            foreach (string fileName in _fileNames)
            {
                TextAsset file = Resources.Load<TextAsset>(fileName);
                if (file != null)
                {
                    _processor.LoadFromCSV(file.text, _currentLanguage, fileName);
                }
                else
                {
                    DebugManager.LogWarning($"[LocalizationManager] File not found in Resources: {fileName}");
                }
            }
            EventManager.TriggerEvent<LanguageChangedEvent>(_currentLanguage);
            Resources.UnloadUnusedAssets();
        }

        private void Update()
        {
            if (Keyboard.current != null) 
            {
                bool wasPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
                if (wasPressed) 
                {
                    var newLang = _currentLanguage == ELanguageID.en ? ELanguageID.sp : ELanguageID.en;
                    ChangeLanguage(newLang);
                }
            }
        }
        #endregion private
    }
}