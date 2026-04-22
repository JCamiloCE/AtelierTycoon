using JCC.LifeCycle;
using JCC.Debug;
using UnityEngine;

namespace JCC.Utils.Art
{
    public class SpriteColorChanger : IColorChanger
    {
        private bool _wasInitialized = false;
        private SpriteRenderer _spriteRender;

        public bool WasInitialized() => _wasInitialized;

        bool ILifeCycle.Initialization(params object[] parameters)
        {
            _spriteRender = parameters[0] as SpriteRenderer;
            _wasInitialized = _spriteRender != null ? true : false;
            return _wasInitialized;
        }

        Color IColorChanger.GetColor()
        {
            if (!WasInitialized())
            {
                DebugManager.LogError("SpriteColorChanger was not initialized");
                return Color.white;
            }
            return _spriteRender.color;
        }

        void IColorChanger.SetColor(Color color)
        {
            if (!WasInitialized())
            {
                DebugManager.LogError("SpriteColorChanger was not initialized");
                return;
            }
            _spriteRender.color = color;
        }
    }
}
