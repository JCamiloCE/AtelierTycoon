using CustomEvent = Unity.Services.Analytics.Event;

namespace Emc2.Scripts.Analytics
{
    public class BaseAnalyticsEvent : CustomEvent
    {
        public BaseAnalyticsEvent(string name) : base(name)
        {
        }
    }
}