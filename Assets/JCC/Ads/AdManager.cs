using JCC.Debug;
using System;
using UnityEngine;

namespace JCC.Ads
{
    public class AdManager : MonoBehaviour
    {
        public event Action<ETypeAd> OnAdShowStarted;
        public event Action<ETypeAd, bool> OnAdShowCompleted;
        public event Action<ETypeAd> OnAdShowFailed;

        private Action<bool> _onInitialized = null;
        private IAdProvider _adProvider = null;
        private AdUnit _interstitialAd = null;
        private AdUnit _rewardedAd = null;

        #region public
        public void InitializeAds(IAdProvider adProvider, Action<bool> onInitialized, bool consent, bool isDebugBuild)
        {
            _onInitialized = onInitialized;
            _adProvider = adProvider;
            _interstitialAd = new AdUnit(ETypeAd.Interstitial, _adProvider);
            _rewardedAd = new AdUnit(ETypeAd.Reward, _adProvider);
            _adProvider.ConfigureGDPR(consent);
            _adProvider.Initialize(isDebugBuild, OnInitialized);
        }

        public bool TryShowInterstitial()
        {
            if (!_interstitialAd.TryShow())
            {
                DebugManager.LogWarning($"[AdManager] ShowInterstitialAdEvent: Interstitial not ready.");
                LoadInterstitialInternal();
                return false;
            }
            LoadInterstitialInternal();
            return true;
        }

        public bool TryShowRewarded()
        {
            if (!_rewardedAd.TryShow())
            {
                DebugManager.LogWarning($"[AdManager] ShowRewardedAdEvent: Rewarded not ready.");
                LoadRewardedInternal();
                return false;
            }
            LoadRewardedInternal();
            return true;
        }
        #endregion public

        #region private
        private void OnDestroy()
        {
            CancelInvoke();
        }

        private void OnInitialized(bool wasInitialized, string error) 
        {
            if (wasInitialized)
            {
                SuscribeToProvider();
                LoadAllAds();
            }
            else 
            {
                DebugManager.LogWarning($"OnInitializationFailed: {error.ToString()}");
                DebugManager.LogWarning($"[AdManager] OnInitialized typeAd: {error.ToString()}");
            }
            _onInitialized?.Invoke(wasInitialized);
        }

        private void SuscribeToProvider() 
        {
            _adProvider.OnAdLoaded += AdLoaded;
            _adProvider.OnAdLoadFailed += AdLoadFailed;
            _adProvider.OnAdShowStarted += AdShowStarted;
            _adProvider.OnAdShowCompleted += AdShowCompleted;
            _adProvider.OnAdShowFailed += AdShowFailed;
        }

        private void LoadAllAds()
        {
            LoadInterstitialInternal();
            LoadRewardedInternal();
        }

        private void LoadInterstitialInternal() => _interstitialAd.Load();
        private void LoadRewardedInternal() => _rewardedAd.Load();
        #endregion private

        private void AdLoaded(ETypeAd typeAd) 
        {
            switch (typeAd)
            { 
                case ETypeAd.Interstitial:
                    _interstitialAd.SetState(EAdState.Loaded);
                    break;

                case ETypeAd.Reward:
                    _rewardedAd.SetState(EAdState.Loaded);
                    break;
            }
        }

        private void AdLoadFailed(ETypeAd typeAd, string message) 
        {
            DebugManager.LogWarning($"[AdManager] OnAdLoadFailed typeAd: {typeAd} - message: {message}");
            switch (typeAd)
            {
                case ETypeAd.Interstitial:
                    _interstitialAd.SetState(EAdState.None);
                    CancelInvoke(nameof(LoadInterstitialInternal));
                    Invoke(nameof(LoadInterstitialInternal), 5f);
                    break;

                case ETypeAd.Reward:
                    _rewardedAd.SetState(EAdState.None);
                    CancelInvoke(nameof(LoadRewardedInternal));
                    Invoke(nameof(LoadRewardedInternal), 5f);
                    break;
            }
        }

        private void AdShowStarted(ETypeAd typeAd) 
        {
            OnAdShowStarted?.Invoke(typeAd);
        }

        private void AdShowCompleted(ETypeAd typeAd, bool wasCompleted) 
        {
            switch (typeAd)
            {
                case ETypeAd.Interstitial:
                    _interstitialAd.SetState(EAdState.None);
                    LoadInterstitialInternal();
                    break;

                case ETypeAd.Reward:
                    
                    _rewardedAd.SetState(EAdState.None);
                    LoadRewardedInternal();
                    break;
            }
            OnAdShowCompleted?.Invoke(typeAd, wasCompleted);
        }

        private void AdShowFailed(ETypeAd typeAd, string message) 
        {
            DebugManager.LogWarning($"[AdManager] OnAdShowFailed typeAd: {typeAd} - message: {message}");
            switch (typeAd)
            {
                case ETypeAd.Interstitial:
                    _interstitialAd.SetState(EAdState.None);
                    LoadInterstitialInternal();
                    break;

                case ETypeAd.Reward:
                    _rewardedAd.SetState(EAdState.None);
                    LoadRewardedInternal();
                    break;
            }
            OnAdShowFailed?.Invoke(typeAd);
        }
    }
}