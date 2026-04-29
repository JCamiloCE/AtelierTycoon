using CustomEvent = Unity.Services.Analytics.Event;

namespace Emc2.Scripts.Analytics
{
    public class AdCompletedAnalyticsEvent : CustomEvent
    {
        public AdCompletedAnalyticsEvent(string name) : base(name)
        {
        }

        public string AdType { set { SetParameter("adType", value); } }
    }
}
