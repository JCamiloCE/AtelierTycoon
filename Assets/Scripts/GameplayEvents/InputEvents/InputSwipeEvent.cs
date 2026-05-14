using JCC.GameplayEventSystem;

namespace Emc2.Scripts.GameplayEvents
{
    public class InputSwipeEvent : EventBase
    {
        public float SwipeDeltaX { get; private set; }

        public override void SetParameters(params object[] parameters)
        {
            SwipeDeltaX = (float)parameters[0];
        }
    }
}
