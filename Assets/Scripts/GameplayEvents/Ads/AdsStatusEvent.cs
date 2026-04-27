using JCC.Enums;
using JCC.GameplayEventSystem;
using System.Collections.Generic;

namespace Emc2.Scripts.GameplayEvents
{
    public class AdsStatusEvent : EventBase
    {
        public Dictionary<ETypeAd, bool> adsStatus;

        public override void SetParameters(params object[] parameters)
        {
            adsStatus = (Dictionary<ETypeAd, bool>)parameters[0];
        }
    }
}
