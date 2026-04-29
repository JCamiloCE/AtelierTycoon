using Emc2.Scripts.Enums;
using JCC.GameplayEventSystem;

namespace Emc2.Scripts.GameplayEvents
{
    public class ShowRewardedAdEvent : EventBase
    {
        public ERewardAds rewardAds;

        public override void SetParameters(params object[] parameters)
        {
            rewardAds = (ERewardAds)parameters[0];
        }
    }
}