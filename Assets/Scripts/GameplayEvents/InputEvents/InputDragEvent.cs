using JCC.GameplayEventSystem;
using UnityEngine;

namespace Emc2.Scripts.GameplayEvents
{
    public class InputDragEvent : EventBase
    {
        public Vector3 InitScreenPosition    { get; private set; }
        public Vector3 CurrentScreenPosition { get; private set; }

        public override void SetParameters(params object[] parameters)
        {
            InitScreenPosition    = (Vector3)parameters[0];
            CurrentScreenPosition = (Vector3)parameters[1];
        }
    }
}
