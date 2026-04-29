using JCC.Debug;
using JCC.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JCC.Ads
{
    public class AdManager : MonoBehaviour
    {
        private const string SHOWED_VIDEO_KEY = "ShowedVideoPlayerPrefKey";

        public event Action<ETypeAd> OnAdShowStarted;
        public event Action<ETypeAd, bool> OnAdShowCompleted;
        public event Action<ETypeAd> OnAdShowFailed;
        public event Action OnAdLoaded;

        private Action<bool> _onInitialized = null;
        private IAdProvider _adProvider = null;
        private AdUnit _interstitialAd = null;
        private AdUnit _rewardedAd = null;
        private int _maxCounterToInterstitial = 0;
        private int _currentToInterstitial = 0;

        #region public
        public void InitializeAds(IAdProvider adProvider, Action<bool> onInitialized, int counterToInterstitial, bool consent, bool isDebugBuild)
        {
            _onInitialized = onInitialized;
            _adProvider = adProvider;
            _interstitialAd = new AdUnit(ETypeAd.Interstitial, _adProvider);
            _rewardedAd = new AdUnit(ETypeAd.Reward, _adProvider);
            _adProvider.ConfigureGDPR(consent);
            _adProvider.Initialize(isDebugBuild, OnInitialized);
            _maxCounterToInterstitial = counterToInterstitial;
            _currentToInterstitial = 0;
            if (PlayerPrefs.HasKey(SHOWED_VIDEO_KEY))
            {
                _currentToInterstitial = PlayerPrefs.GetInt(SHOWED_VIDEO_KEY);
            }
        }

        public bool TryShowInterstitial()
        {
            SetCoutnerInterstitialMax(_currentToInterstitial - 1);
            if (_currentToInterstitial <= 0)
            {
                if (!_interstitialAd.TryShow())
                {
                    DebugManager.LogWarning($"[AdManager] ShowInterstitialAdEvent: Interstitial not ready.");
                    LoadInterstitialInternal();
                    SetCoutnerInterstitialMax(_currentToInterstitial + 1);
                    return false;
                }
                LoadInterstitialInternal();
                SetCoutnerInterstitialMax(_maxCounterToInterstitial);
                return true;
            }
            return false;
        }

        public bool TryShowRewarded()
        {
            if (!_rewardedAd.TryShow())
            {
                DebugManager.LogWarning($"[AdManager] ShowRewardedAdEvent: Rewarded not ready.");
                LoadRewardedInternal();
                return false;
            }
            SetCoutnerInterstitialMax(_maxCounterToInterstitial);
            LoadRewardedInternal();
            return true;
        }

        public Dictionary<ETypeAd, bool> GetAdsStatus() 
        {
            Dictionary<ETypeAd, bool> adsStatus = new Dictionary<ETypeAd, bool>();
            adsStatus.Add(ETypeAd.Reward, _rewardedAd.GetAdState == EAdState.Loaded);
            adsStatus.Add(ETypeAd.Interstitial, _interstitialAd.GetAdState == EAdState.Loaded);
            return adsStatus;
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
                DebugManager.LogVerbose("[AdManager] Provider was initialized properly");
                SuscribeToProvider();
                LoadAllAds();
            }
            else 
            {
                DebugManager.LogWarning($"[AdManager] OnInitializationFailed: {error.ToString()}");
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
            OnAdLoaded?.Invoke();
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

        private void SetCoutnerInterstitialMax(int newValue) 
        {
            _currentToInterstitial = newValue;
            PlayerPrefs.SetInt(SHOWED_VIDEO_KEY, _currentToInterstitial);
            PlayerPrefs.Save();
        }
    }
}