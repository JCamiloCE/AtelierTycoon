using Emc2.Scripts.Enums;
using Emc2.Scripts.Scenes;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.UnityConsent;

namespace Emc2.Scripts.GeneralManagers
{
    public class PrivacyConsentManager : MonoBehaviour
    {
        private const string CONSENT_KEY = "user_consent_v1";
        private static string DEV = "development";
        private static string PROD = "production";

        [SerializeField] private GameObject _consentPopUp = null;

        private bool _isDebugBuild = false;

        #region public
        public void OnAccept()
        {
            PlayerPrefs.SetInt(CONSENT_KEY, 1);
            SetActiveConsentPopup(false);
            ApplyConsent(true);
        }

        public void OnDecline()
        {
            PlayerPrefs.SetInt(CONSENT_KEY, 0);
            SetActiveConsentPopup(false);
            ApplyConsent(false);
        }

        public void OpenLinkPolicy() 
        {
            Application.OpenURL("https://docs.google.com/document/d/1q-lu7meFykkXs8MkHMXpeJTgy2O4n1I5f4NYkClflNo/edit?usp=sharing");
        }
        #endregion public

        #region private
        private async void Start()
        {
            _isDebugBuild = UnityEngine.Debug.isDebugBuild;

            var options = new InitializationOptions();
            options.SetEnvironmentName(_isDebugBuild ? DEV : PROD);
            await UnityServices.InitializeAsync(options);

            if (PlayerPrefs.HasKey(CONSENT_KEY))
            {
                if (PlayerPrefs.GetInt(CONSENT_KEY) == 1)
                { 
                    ApplyConsent(true);
                    return;
                }
            }

            SetActiveConsentPopup(true);
        }

        private void SetActiveConsentPopup(bool newState)
        {
            _consentPopUp.SetActive(newState);
        }

        private void ApplyConsent(bool consent)
        {
            ConsentState state = new ConsentState();
            state.AnalyticsIntent = consent ? ConsentStatus.Granted : ConsentStatus.Denied;
            state.AdsIntent = consent ? ConsentStatus.Granted : ConsentStatus.Denied;
            EndUserConsent.SetConsentState(state);

            //AdController adController = new AdController();
            //adController.InitAds(consent, _isDebugBuild);
            //AnalyticsManager analytics = new AnalyticsManager();
            //analytics.Initialization(consent);

            SceneController.Instance.LoadScene(ESceneIds.MainMenu);
        }
        #endregion private
    }
}