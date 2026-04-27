using JCC.Ads;
using JCC.Debug;
using JCC.Enums;
using System;
using UnityEngine.Advertisements;

namespace Emc2.Scripts.AdsImplementation
{
    public class UnityAdsProvider : IUnityAdsInitializationListener, 
                                    IUnityAdsLoadListener, 
                                    IUnityAdsShowListener, 
                                    IAdProvider
    {
        private const string GAME_ID = "-1";
        private const string PLACEMENT_INTERSTITIAL_ID = "Interstitial_Android";
        private const string PLACEMENT_REWARDED_ID = "Rewarded_Android";

        public event Action<ETypeAd> OnAdLoaded;
        public event Action<ETypeAd, string> OnAdLoadFailed;
        public event Action<ETypeAd> OnAdShowStarted;
        public event Action<ETypeAd, bool> OnAdShowCompleted;
        public event Action<ETypeAd, string> OnAdShowFailed;

        private Action<bool, string> _onInitCallback;

        public bool IsInitialized => Advertisement.isInitialized;

        public void Initialize(bool testMode, Action<bool, string> onInitComplete)
        {
            _onInitCallback = onInitComplete;
            Advertisement.Initialize(GAME_ID, testMode, this);
        }

        public void ConfigureGDPR(bool consent)
        {
            MetaData gdprMetaData = new MetaData("gdpr");
            gdprMetaData.Set("consent", consent);
            Advertisement.SetMetaData(gdprMetaData);
        }

        public void LoadAd(ETypeAd typeAd) 
        {
            Advertisement.Load(GetPlacementByTypeAd(typeAd), this);
        }

        public void ShowAd(ETypeAd typeAd)
        {
            Advertisement.Show(GetPlacementByTypeAd(typeAd), this);
        }

        #region Unity Callbacks
        public void OnInitializationComplete() 
            => _onInitCallback?.Invoke(true, string.Empty);

        public void OnInitializationFailed(UnityAdsInitializationError error, string message) 
            => _onInitCallback?.Invoke(false, message);

        public void OnUnityAdsAdLoaded(string placementId) 
            => OnAdLoaded?.Invoke(GetTypeAdByPlacement(placementId));

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) 
            => OnAdLoadFailed?.Invoke(GetTypeAdByPlacement(placementId), message);

        public void OnUnityAdsShowStart(string placementId) 
            => OnAdShowStarted?.Invoke(GetTypeAdByPlacement(placementId));

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) 
            => OnAdShowFailed?.Invoke(GetTypeAdByPlacement(placementId), message);
        
        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState) 
            => OnAdShowCompleted?.Invoke(GetTypeAdByPlacement(placementId), showCompletionState == UnityAdsShowCompletionState.COMPLETED);

        public void OnUnityAdsShowClick(string placementId) 
        {
            //NOOP
        }
        #endregion

        private string GetPlacementByTypeAd(ETypeAd typeAd) 
        {
            switch (typeAd)
            {
                case ETypeAd.Interstitial:
                    return PLACEMENT_INTERSTITIAL_ID;
                case ETypeAd.Reward:
                    return PLACEMENT_REWARDED_ID;
            }

            DebugManager.LogError($"[UnityAdsProvider] GetPlacementByTypeAd: Incorrect Type Ad: {typeAd}");
            return string.Empty;
        }

        private ETypeAd GetTypeAdByPlacement(string placement) 
        {
            if (placement == PLACEMENT_INTERSTITIAL_ID) 
            {
                return ETypeAd.Interstitial;
            }

            if (placement == PLACEMENT_REWARDED_ID)
            { 
                return ETypeAd.Reward;
            }

            DebugManager.LogError($"[UnityAdsProvider] GetTypeAdByPlacement: Incorrect placement: {placement}");
            return ETypeAd.None;
        }
    }
}