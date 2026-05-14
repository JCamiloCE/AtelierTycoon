using Emc2.Scripts.Enums;
using JCC.GameplayEventSystem;

namespace Emc2.Scripts.GameplayEvents
{
    public class ViewChangedEvent : EventBase
    {
        public EAtelierView View { get; private set; }

        public override void SetParameters(params object[] parameters)
        {
            View = (EAtelierView)parameters[0];
        }
    }
}
