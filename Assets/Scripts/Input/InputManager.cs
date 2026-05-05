using JCC.Debug;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Emc2.Scripts.Input
{
    public class InputManager : MonoBehaviour
    {
        [Header("Mobile Values")]
        [SerializeField] private float _zoomSpeedMobile;
        [Header("PC Values")]
        [SerializeField] private float _zoomSpeedPc;
        [Header("General Values")]
        [SerializeField] private float _minOrthographicSize;
        [SerializeField] private float _maxOrthographicSize;
        [Header("Temp DELETE THIS")]
        [SerializeField] private Color _left;
        [SerializeField] private Color _right;
        [SerializeField] private SpriteRenderer _temp;

        private Vector3 _cameraOriginPos;
        private Vector3 _lastInitPos;
        private float _zoomSpeed;

        private IInput _currentInput;
        private Camera _camera;

        #region Private
        private void Awake()
        {
            _camera = Camera.main;

            if (Application.isEditor)
            {
                _currentInput = new PCInput();
                _zoomSpeed = _zoomSpeedPc;
            }
            else 
            {
                _currentInput = new MobileInput();
                _zoomSpeed = _zoomSpeedMobile;
            }
            _cameraOriginPos = _camera.transform.position;
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

        private void Swipe(float deltaX)
        {
            _temp.color = deltaX > 0 ? _left : _right;
        }

        private void Zoom(float zoomVal)
        {
            float newSize = _camera.orthographicSize - (zoomVal * _zoomSpeed * Time.deltaTime);
            _camera.orthographicSize = Mathf.Clamp(newSize, _minOrthographicSize, _maxOrthographicSize);
        }

        private void Drag(Vector3 initScreenPosition, Vector3 currentScreenPosition)
        {
            if (!AreEqual(initScreenPosition, _lastInitPos))
            {
                _lastInitPos = initScreenPosition;
                _cameraOriginPos = _camera.transform.position;
            }
            Vector3 worldStart = _camera.ScreenToWorldPoint(new Vector3(initScreenPosition.x, initScreenPosition.y, 10f));
            Vector3 worldCurrent = _camera.ScreenToWorldPoint(new Vector3(currentScreenPosition.x, currentScreenPosition.y, 10f));
            Vector3 difference = worldStart - worldCurrent;
            _camera.transform.position = _cameraOriginPos + new Vector3(difference.x, difference.y, 0);
        }

        private bool AreEqual(Vector3 a, Vector3 b, float epsilon = 0.00001f)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;
            return (dx * dx + dy * dy + dz * dz) < (epsilon * epsilon);
        }
        #endregion Private
    }
}