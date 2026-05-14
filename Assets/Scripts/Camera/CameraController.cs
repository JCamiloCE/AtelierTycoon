using Emc2.Scripts.Enums;
using Emc2.Scripts.GameplayEvents;
using JCC.Debug;
using JCC.Fade;
using JCC.GameplayEventSystem;
using UnityEngine;

namespace Emc2.Scripts.Camera
{
    public class CameraController : MonoBehaviour,
                                    IEventListener<InputZoomEvent>,
                                    IEventListener<InputDragEvent>,
                                    IEventListener<InputSwipeEvent>
    {
        [Header("General config")]
        [SerializeField] private UnityEngine.Camera _camera;
        [SerializeField] private CameraViewDataScriptable _viewData;
        [SerializeField] private EAtelierView _initialView = EAtelierView.Facade;

        [Header("Zoom Speed")]
        [SerializeField] private float _zoomSpeedMobile;
        [SerializeField] private float _zoomSpeedPc;

        [Header("Fade")]
        [SerializeField] private float _fadeDuration = 0.3f;

        private float _zoomSpeed;
        private CameraViewDataScriptable.ViewEntry _currentEntry;
        private EAtelierView _currentView;
        private ViewNavigator _navigator;
        private Vector3 _cameraOriginPos;
        private Vector3 _lastDragInitPos;
        private bool _dragInitialized;
        private bool _isTransitioning;

        #region MonoBehaviour
        private void Awake()
        {
            if (_camera == null || _viewData == null)
            {
                DebugManager.LogError("[CameraController] Missing Camera or ViewData reference. Disabling.");
                enabled = false;
                return;
            }

            _zoomSpeed = Application.isEditor ? _zoomSpeedPc : _zoomSpeedMobile;
            _navigator = new ViewNavigator(_viewData.ViewOrder);
            ApplyView(_initialView);
        }

        private void OnEnable()
        {
            EventManager.AddListener<InputZoomEvent>(this);
            EventManager.AddListener<InputDragEvent>(this);
            EventManager.AddListener<InputSwipeEvent>(this);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<InputZoomEvent>(this);
            EventManager.RemoveListener<InputDragEvent>(this);
            EventManager.RemoveListener<InputSwipeEvent>(this);
        }
        #endregion MonoBehaviour

        #region IEventListener
        public void OnEvent(InputZoomEvent eventData)
        {
            if (_isTransitioning || _currentEntry == null) 
            {
                return;
            }

            float newSize = _camera.orthographicSize - (eventData.ZoomDelta * _zoomSpeed * Time.deltaTime);
            _camera.orthographicSize = Mathf.Clamp(newSize, _currentEntry.minOrthographicSize, _currentEntry.maxOrthographicSize);
        }

        public void OnEvent(InputDragEvent eventData)
        {
            if (_isTransitioning || _currentEntry == null)
            {
                return;
            }

            if (!_dragInitialized || !AreEqual(eventData.InitScreenPosition, _lastDragInitPos))
            {
                _lastDragInitPos = eventData.InitScreenPosition;
                _cameraOriginPos = _camera.transform.position;
                _dragInitialized = true;
            }

            float camZ = Mathf.Abs(_camera.transform.position.z);
            Vector3 worldStart = _camera.ScreenToWorldPoint(new Vector3(eventData.InitScreenPosition.x, eventData.InitScreenPosition.y, camZ));
            Vector3 worldCurrent = _camera.ScreenToWorldPoint(new Vector3(eventData.CurrentScreenPosition.x, eventData.CurrentScreenPosition.y, camZ));
            Vector3 target = _cameraOriginPos + (worldStart - worldCurrent);
            target.x = Mathf.Clamp(target.x, _currentEntry.minBounds.x, _currentEntry.maxBounds.x);
            target.y = Mathf.Clamp(target.y, _currentEntry.minBounds.y, _currentEntry.maxBounds.y);
            target.z = _camera.transform.position.z;
            _camera.transform.position = target;
        }

        public void OnEvent(InputSwipeEvent eventData)
        {
            if (_isTransitioning || _currentEntry == null)
            {
                return;
            }

            int direction = eventData.SwipeDeltaX > 0f ? 1 : -1;
            EAtelierView? next = _navigator.GetAdjacent(_currentView, direction);
            if (next == null)
            {
                return;
            }

            ChangeView(next.Value);
        }
        #endregion IEventListener

        #region Private
        private void ChangeView(EAtelierView next)
        {
            _isTransitioning = true;
            FadeController.Instance.FadeIn(_fadeDuration, true, false, () =>
            {
                ApplyView(next);
                EventManager.TriggerEvent<ViewChangedEvent>((object)next);
                FadeController.Instance.FadeOut(_fadeDuration, false, false, () => _isTransitioning = false);
            });
        }

        private void ApplyView(EAtelierView view)
        {
            CameraViewDataScriptable.ViewEntry entry = _viewData.GetDataByView(view);
            if (entry == null) return;

            _currentView = view;
            _currentEntry = entry;

            float z = _camera.transform.position.z;
            _camera.transform.position = new Vector3(entry.defaultPosition.x, entry.defaultPosition.y, z);
            _camera.orthographicSize = Mathf.Clamp(entry.defaultZoom, entry.minOrthographicSize, entry.maxOrthographicSize);

            _dragInitialized = false;
        }

        private static bool AreEqual(Vector3 a, Vector3 b, float epsilon = 0.00001f)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;
            return (dx * dx + dy * dy + dz * dz) < (epsilon * epsilon);
        }
        #endregion Private
    }
}
