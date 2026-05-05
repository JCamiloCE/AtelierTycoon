using JCC.Debug;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Emc2.Scripts.Input
{
    public class PCInput : IInput
    {
        private const float edgeThreshold = 0.1f;
        private const float fastSwipeThreshold = 50f; // Velocidad del mouse

        private Vector2 lastMousePosition;
        private Vector2 startClickPos;
        private bool isDragging = false;
        private bool isEdgeActionPossible = false;

        private Action<float> _swipe;
        private Action<float> _zoom;
        private Action<Vector3, Vector3> _drag;

        #region IInput
        public void ActiveInput()
        {
            //NOOP
        }

        public void DeactiveInput()
        {
            //NOOP
        }

        public void SetCallbacks(Action<float> swipe, Action<float> zoom, Action<Vector3, Vector3> drag)
        {
            _swipe = swipe;
            _zoom = zoom;
            _drag = drag;
        }

        public void ProcessInput()
        {
            HandleMouseDrag();
            HandleMouseZoom();
        }
        #endregion IInput

        #region Private 
        private void HandleMouseDrag()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                isDragging = true;
                startClickPos = Mouse.current.position.ReadValue();
                lastMousePosition = startClickPos;

                float screenWidth = Screen.width;
                if (startClickPos.x < screenWidth * edgeThreshold ||
                    startClickPos.x > screenWidth * (1 - edgeThreshold))
                {
                    isEdgeActionPossible = true;
                }
                else
                {
                    isEdgeActionPossible = false;
                }
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                isDragging = false;
                isEdgeActionPossible = false;
            }

            if (isDragging)
            {
                Vector2 currentMousePos = Mouse.current.position.ReadValue();
                Vector2 mouseDelta = currentMousePos - lastMousePosition;

                if (isEdgeActionPossible && mouseDelta.magnitude > fastSwipeThreshold)
                {
                    _swipe.Invoke(mouseDelta.x);
                    isEdgeActionPossible = false; 
                    isDragging = false; 
                }
                else if (!isEdgeActionPossible)
                {
                    _drag.Invoke(startClickPos, currentMousePos);
                }

                lastMousePosition = currentMousePos;
            }
        }

        private void HandleMouseZoom()
        {
            float scrollValue = Mouse.current.scroll.ReadValue().y;

            if (Mathf.Abs(scrollValue) > 0.01f)
            {
                _zoom.Invoke(scrollValue);
            }
        }
        #endregion Private
    }
}