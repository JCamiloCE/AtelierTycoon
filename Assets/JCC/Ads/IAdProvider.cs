using System;

namespace JCC.Ads
{
    public interface IAdProvider
    {
        void Initialize(bool testMode, Action<bool, string> onInitComplete);
        void ConfigureGDPR(bool consent);
        void LoadAd(ETypeAd typeAd);
        void ShowAd(ETypeAd typeAd);
        bool IsInitialized { get; }

        event Action<ETypeAd> OnAdLoaded;
        event Action<ETypeAd, string> OnAdLoadFailed;
        event Action<ETypeAd> OnAdShowStarted;
        event Action<ETypeAd, bool> OnAdShowCompleted;
        event Action<ETypeAd, string> OnAdShowFailed;
    }
}