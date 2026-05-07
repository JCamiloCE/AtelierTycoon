using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Emc2.Scripts.Input
{
    public class MobileInput : IInput
    {
        private const float EDGE_THRESHOLD = 0.1f;
        private const float FAST_SWIPE_THRESHOLD = 50f;

        private Vector2 startTouchPos;
        private bool isEdgeActionPossible = false;

        private Action<float> _swipe;
        private Action<float> _zoom;
        private Action<Vector3, Vector3> _drag;

        #region IInput
        public void ActiveInput()
        {
            EnhancedTouchSupport.Enable();
        }

        public void DeactiveInput()
        {
            EnhancedTouchSupport.Disable();
        }

        public void SetCallbacks(Action<float> swipe, Action<float> zoom, Action<Vector3, Vector3> drag)
        {
            _swipe = swipe;
            _zoom = zoom;
            _drag = drag;
        }

        public void ProcessInput()
        {
            HandleDrag();
            HandleZoom();
        }
        #endregion IInput

        #region Private
        public void HandleDrag()
        {
            if (Touch.activeTouches.Count == 1)
            {
                var touch = Touch.activeTouches[0];

                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    startTouchPos = touch.screenPosition;
                    float screenWidth = Screen.width;

                    if (startTouchPos.x < screenWidth * EDGE_THRESHOLD ||
                        startTouchPos.x > screenWidth * (1 - EDGE_THRESHOLD))
                    {
                        isEdgeActionPossible = true;
                    }
                    else
                    {
                        isEdgeActionPossible = false;
                    }
                    
                }

                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
                {
                    if (isEdgeActionPossible && touch.delta.magnitude > FAST_SWIPE_THRESHOLD)
                    {
                        _swipe.Invoke(touch.delta.x);
                        isEdgeActionPossible = false;
                    }
                    else if (!isEdgeActionPossible)
                    {
                        _drag.Invoke(startTouchPos, touch.screenPosition);
                    }
                }
            }
        }

        private void HandleZoom()
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
                _zoom.Invoke(deltaDist);
            }
        }
        #endregion Private
    }
}