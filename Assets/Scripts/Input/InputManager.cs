using Emc2.Scripts.GameplayEvents;
using JCC.GameplayEventSystem;
using UnityEngine;

namespace Emc2.Scripts.Input
{
    public class InputManager : MonoBehaviour
    {
        private IInput _currentInput;

        #region Private
        private void Awake()
        {
            if (Application.isEditor)
            {
                _currentInput = new PCInput();
            }
            else
            {
                _currentInput = new MobileInput();
            }
            _currentInput.SetCallbacks(Swipe, Zoom, Drag);
        }

        private void OnEnable()
        {
            _currentInput.ActiveInput();
        }

        private void OnDisable()
        {
            _currentInput.DeactiveInput();
        }

        private void Update()
        {
            _currentInput.ProcessInput();
        }

        private void Swipe(float deltaX) => EventManager.TriggerEvent<InputSwipeEvent>(deltaX);

        private void Zoom(float zoomVal) => EventManager.TriggerEvent<InputZoomEvent>(zoomVal);

        private void Drag(Vector3 initScreenPosition, Vector3 currentScreenPosition)
            => EventManager.TriggerEvent<InputDragEvent>(initScreenPosition, currentScreenPosition);
        #endregion Private
    }
}
