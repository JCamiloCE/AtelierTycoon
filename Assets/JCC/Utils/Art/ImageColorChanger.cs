using JCC.LifeCycle;
using UnityEngine;
using UnityEngine.UI;

namespace JCC.Utils.Art
{
    public class ImageColorChanger : IColorChanger
    {
        private bool _wasInitialized = false;
        private Image _image;

        public bool WasInitialized() => _wasInitialized;

        bool ILifeCycle.Initialization(params object[] parameters)
        {
            _image = parameters[0] as Image;
            _wasInitialized = _image != null ? true : false;
            return _wasInitialized;
        }

        Color IColorChanger.GetColor()
        {
            if (!WasInitialized())
            {
                //DebugManager.LogError();
                return Color.white;
            }
            return _image.color;
        }

        void IColorChanger.SetColor(Color color)
        {
            if (!WasInitialized())
            {
                //DebugManager.LogError();
                return;
            }
            _image.color = color;
        }
    }
}