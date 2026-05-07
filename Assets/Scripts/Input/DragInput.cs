using System;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Emc2.Scripts.Input
{
    public class DragInput : IGestureInput
    {
        [Header("Gesto Especial (Borde)")]
        [Range(0, 0.2f)] [SerializeField] private float edgeThreshold = 0.1f;
        [SerializeField] private float fastSwipeThreshold = 50f;

        private Vector2 startTouchPos;
        private bool isEdgeActionPossible = false;

        #region IGestureInput
        public void ProcessGesture()
        {
            if (Touch.activeTouches.Count == 1)
            {
                var touch = Touch.activeTouches[0];

                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    startTouchPos = touch.screenPosition;
                    float screenWidth = Screen.width;

                    if (startTouchPos.x < screenWidth * edgeThreshold ||
                        startTouchPos.x > screenWidth * (1 - edgeThreshold))
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
                    if (isEdgeActionPossible && touch.delta.magnitude > fastSwipeThreshold)
                    {
                        // Hacer Cambio de escenario
                        isEdgeActionPossible = false;
                    }
                    else if (!isEdgeActionPossible)
                    {
                        // Mover Camara
                    }
                }
            }
        }
        #endregion IGestureInput
    }
}