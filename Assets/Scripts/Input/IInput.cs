using System;
using UnityEngine;

namespace Emc2.Scripts.Input
{
    public interface IInput
    {
        public void ActiveInput();
        public void DeactiveInput();
        public void ProcessInput();
        public void SetCallbacks(Action<float> swipe, Action<float> zoom, Action<Vector3, Vector3> drag);
    }
}