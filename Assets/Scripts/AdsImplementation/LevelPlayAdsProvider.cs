using JCC.Ads;
using JCC.Enums;
using System;
using System.Collections.Generic;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace Emc2.Scripts.AdsImplementation
{
    public class LevelPlayAdsProvider : IAdProvider
    {
        private const string APP_KEY = "26296d54d";
        private Action<bool, string> _onInitCallback;
        private bool _isInitialized;
        private bool _isTestMode;
        private LevelPlayRewardedAd _rewardedAd;

        #region IAdProvider
        public event Action<ETypeAd> OnAdLoaded;
        public event Action<ETypeAd, string> OnAdLoadFailed;
        public event Action<ETypeAd> OnAdShowStarted;
        public event Action<ETypeAd, bool> OnAdShowCompleted;
        public event Action<ETypeAd, string> OnAdShowFailed;

        bool IAdProvider.IsInitialized => _isInitialized;

        void IAdProvider.ConfigureGDPR(bool consent)
        {
            Dictionary<string, bool> consents = new Dictionary<string, bool>
            {
                { "UnityAds", true  },
                { "AdMob",    false },
                { "AppLovin", true  }
            };
            LevelPlayPrivacySettings.SetGDPRConsents(consents);
        }

        void IAdProvider.Initialize(bool testMode, Action<bool, string> onInitComplete)
        {
            _isInitialized = false;
            _isTestMode = testMode;
            _onInitCallback = onInitComplete;
            LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
            LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
            if (_isTestMode)
            {
                LevelPlay.SetMetaData("is_test_suite", "enable");
            }
            LevelPlay.Init(APP_KEY);
        }

        void IAdProvider.LoadAd(ETypeAd typeAd)
        {
            _rewardedAd.LoadAd();
        }

        void IAdProvider.ShowAd(ETypeAd typeAd)
        {
            if (_rewardedAd.IsAdReady())
            {
                _rewardedAd.ShowAd();
            }
        }
        #endregion IAdProvider

        #region LevelPlayCallbacks
        private void SdkInitializationCompletedEvent(LevelPlayConfiguration levelPlayConfiguration)
        {
            _rewardedAd = new LevelPlayRewardedAd("edom2x7v9m2m3zxz");
            RegisterEvents();
            _onInitCallback?.Invoke(true, string.Empty);
            _isInitialized = true;
            if (_isTestMode)
            {
                //LevelPlay.LaunchTestSuite();
            }
        }

        private void SdkInitializationFailedEvent(LevelPlayInitError levelPlayInitError)
        {
            _onInitCallback?.Invoke(false, "Error Code: " + levelPlayInitError.ErrorCode + " -- Message: " + levelPlayInitError.ErrorMessage);
            //TODO try again later
        }
        #endregion LevelPlayCallbacks

        #region Private
        private void RegisterEvents() 
        {
            _rewardedAd.OnAdLoaded += RewardedOnAdLoadedEvent;
            _rewardedAd.OnAdLoadFailed += RewardedOnAdLoadFailedEvent;
            _rewardedAd.OnAdDisplayed += RewardedOnAdDisplayedEvent;
            _rewardedAd.OnAdDisplayFailed += RewardedOnAdDisplayFailedEvent;
            _rewardedAd.OnAdRewarded += RewardedOnAdRewardedEvent;
        }


        private void RewardedOnAdLoadedEvent(LevelPlayAdInfo adInfo) => OnAdLoaded?.Invoke(ETypeAd.Reward);
        private void RewardedOnAdLoadFailedEvent(LevelPlayAdError adError) => OnAdLoadFailed?.Invoke(ETypeAd.Reward, adError.ErrorMessage);
        private void RewardedOnAdDisplayedEvent(LevelPlayAdInfo adInfo) => OnAdShowStarted?.Invoke(ETypeAd.Reward);
        private void RewardedOnAdDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError adError) => OnAdShowFailed?.Invoke(ETypeAd.Reward, adError.ErrorMessage);
        private void RewardedOnAdRewardedEvent(LevelPlayAdInfo adInfo, LevelPlayReward rewarded) => OnAdShowCompleted?.Invoke(ETypeAd.Reward, true);
        #endregion Private
    }
}