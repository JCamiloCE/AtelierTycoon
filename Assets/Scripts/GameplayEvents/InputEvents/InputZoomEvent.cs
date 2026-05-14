using JCC.GameplayEventSystem;

namespace Emc2.Scripts.GameplayEvents
{
    public class InputZoomEvent : EventBase
    {
        public float ZoomDelta { get; private set; }

        public override void SetParameters(params object[] parameters)
        {
            ZoomDelta = (float)parameters[0];
        }
    }
}
