using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Emc2.Scripts.Input
{
    public class PinchInput : IGestureInput
    {
        #region IGestureInput
        public void ProcessGesture()
        {
            if (Touch.activeTouches.Count == 2)
            {
                var touch0 = Touch.activeTouches[0];
                var touch1 = Touch.activeTouches[1];

                Vector2 pos0 = touch0.screenPosition;
                Vector2 pos1 = touch1.screenPosition;
                Vector2 prevPos0 = pos0 - touch0.delta;
                Vector2 prevPos1 = pos1 - touch1.delta;

                float currentDist = Vector2.Distance(pos0, pos1);
                float prevDist = Vector2.Distance(prevPos0, prevPos1);

                float deltaDist = prevDist - currentDist;

                //Cambiar orthographicSize
                //cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + deltaDist * zoomSpeed, minOrthographicSize, maxOrthographicSize);
            }
        }
        #endregion IGestureInput
    }
}