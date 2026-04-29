using Emc2.Scripts.Enums;
using Emc2.Scripts.GameplayEvents;
using JCC.Debug;
using JCC.GameplayEventSystem;
using System.Collections.Generic;
using Unity.Services.Analytics;
using CustomEvent = Unity.Services.Analytics.Event;

namespace Emc2.Scripts.Analytics
{
    public class AnalyticsManager : IEventListener<AnalyticsGameplayEvent>
    {
        private bool _consent = false;
        private AnalyticsEventCreator _eventCreator;

        #region IEventListener
        public void OnEvent(AnalyticsGameplayEvent event_data)
        {
            SendEvent(event_data.identifier, event_data.parametersDic);
        }
        #endregion

        #region public
        public void Initialization(bool consent) 
        {
            //Events
            _consent = consent;
            if (_consent)
            {
                _eventCreator = new AnalyticsEventCreator();
                EventManager.AddListener(this);
            }
        }
        #endregion public

        #region private
        private void SendEvent(EAnalityEventIdentifier identifier, Dictionary<string, object> parameters)
        {
            if (!_consent)
            { 
                return;
            }

            CustomEvent customEvent = _eventCreator.CreateCustomEvent(identifier, parameters);

            if (customEvent != null) 
            {
                AnalyticsService.Instance.RecordEvent(customEvent);
                AnalyticsService.Instance.Flush();
                DebugManager.LogVerbose($"[AnalyticsManager] New event sent -> ID: {identifier}");
            }
        }
        #endregion private
    }
}