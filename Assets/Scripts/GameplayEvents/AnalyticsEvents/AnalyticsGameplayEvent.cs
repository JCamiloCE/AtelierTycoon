using Emc2.Scripts.Enums;
using JCC.GameplayEventSystem;
using System.Collections.Generic;

namespace Emc2.Scripts.GameplayEvents
{
    public class AnalyticsGameplayEvent : EventBase
    {
        public EAnalityEventIdentifier identifier;
        public Dictionary<string, object> parametersDic;

        public override void SetParameters(params object[] parameters)
        {
            identifier = (EAnalityEventIdentifier)parameters[0];
            parametersDic = (Dictionary<string, object>)parameters[1];
        }
    }
}
