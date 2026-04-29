using Emc2.Scripts.Enums;
using Emc2.Scripts.GameplayEvents;
using JCC.Ads;
using JCC.Debug;
using JCC.Enums;
using JCC.GameplayEventSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Emc2.Scripts.AdsImplementation
{
    public class AdController : IEventListener<ShowInterstitialAdEvent>,
                                IEventListener<ShowRewardedAdEvent>,
                                IEventListener<RequestAdsStatusEvent>
    {
        private const int MAX_COUNTER_TO_INTERSTITIAL = 3;
        private AdManager _adManager;
        private ERewardAds _rewardToClaim = ERewardAds.None;

        #region IEventListener
        public void OnEvent(ShowInterstitialAdEvent event_data)
        {
            //if (HackManager.GetInstance().GetNoAds())
            //{
            //    return;
            //}

            if (_adManager.TryShowInterstitial())
            {
                EventManager.TriggerEvent<AnalyticsGameplayEvent>(EAnalityEventIdentifier.AdInterstitial, null);
            }
        }

        public void OnEvent(ShowRewardedAdEvent event_data)
        {
            if (event_data.rewardAds == ERewardAds.None)
            {
                DebugManager.LogError("[AdManager] ShowRewardedAdEvent: Rewarded requested with NONE reward");
            }

            _rewardToClaim = event_data.rewardAds;
            if (_adManager.TryShowRewarded())
            {
                EventManager.TriggerEvent<AnalyticsGameplayEvent>(EAnalityEventIdentifier.AdRewarded, null);
            }
            else 
            {
                _rewardToClaim = ERewardAds.None;
            }
        }

        public void OnEvent(RequestAdsStatusEvent event_data)
        {
            SendAdstatusEvent();
        }
        #endregion IEventListener

        #region public
        public void InitAds(bool consent, bool isDebugBuild)
        {
            EventManager.AddListener<ShowInterstitialAdEvent>(this);
            EventManager.AddListener<ShowRewardedAdEvent>(this);
            EventManager.AddListener<RequestAdsStatusEvent>(this);

            IAdProvider adProvider = new LevelPlayAdsProvider();
            GameObject newGoAdManager = new GameObject("AdManager");
            _adManager = newGoAdManager.AddComponent<AdManager>();
            _adManager.InitializeAds(adProvider, OnInitialized, MAX_COUNTER_TO_INTERSTITIAL, consent, isDebugBuild);
            GameObject.DontDestroyOnLoad(_adManager.gameObject);
        }
        #endregion public

        #region private
        private void OnInitialized(bool wasInitialized)
        {
            if (wasInitialized)
            {
                _adManager.OnAdShowStarted += AdShowStarted;
                _adManager.OnAdShowCompleted += AdShowCompleted;
                _adManager.OnAdShowFailed += AdShowFailed;
                _adManager.OnAdLoaded += SendAdstatusEvent;
            }
        }
        #endregion private

        #region Callbacks
        private void AdShowStarted(ETypeAd typeAd) 
        {
            Time.timeScale = 0f;
        }

        private void AdShowCompleted(ETypeAd typeAd, bool wasCompleted)
        {
            Time.timeScale = 1f;
            var dictionary = new Dictionary<string, object>
            {
                { "adType", typeAd.ToString() }
            };
            EventManager.TriggerEvent<AnalyticsGameplayEvent>(EAnalityEventIdentifier.AdCompleted, dictionary);
            if (typeAd == ETypeAd.Reward) 
            {
                if (wasCompleted)
                {
                    EventManager.TriggerEvent<RewardedAdCompletedEvent>(_rewardToClaim);
                }
                _rewardToClaim = ERewardAds.None;
            }
        }

        private void AdShowFailed(ETypeAd typeAd)
        {
            Time.timeScale = 1f;
            if (typeAd == ETypeAd.Reward)
            {
                _rewardToClaim = ERewardAds.None;
            }
        }

        private void SendAdstatusEvent() 
        {
            Dictionary<ETypeAd, bool> adsStatus = _adManager.GetAdsStatus();
            if (adsStatus != null && adsStatus.Count > 0) 
            {
                EventManager.TriggerEvent<AdsStatusEvent>(adsStatus);
            }
        }
        #endregion Callbacks
    }
}