using Emc2.Scripts.Enums;
using System.Collections.Generic;
using UnityEngine.Identifiers;
using CustomEvent = Unity.Services.Analytics.Event;

namespace Emc2.Scripts.Analytics
{
    public class AnalyticsEventCreator
    {
        public CustomEvent CreateCustomEvent(EAnalityEventIdentifier identifier, Dictionary<string, object> parameters)
        {
            switch (identifier)
            {
                case EAnalityEventIdentifier.AdCompleted:
                    return CreateAdCompletedEvent(identifier.ToString(), parameters);
                case EAnalityEventIdentifier.AdInterstitial:
                case EAnalityEventIdentifier.AdRewarded:
                    return new BaseAnalyticsEvent(identifier.ToString());
            }

            return null;
        }

        private CustomEvent CreateAdCompletedEvent(string identifier, Dictionary<string, object> parameters)
        {
            AdCompletedAnalyticsEvent myEvent = new AdCompletedAnalyticsEvent(identifier);
            myEvent.AdType = (string)parameters["adType"];
            return myEvent;
        }
    }
}