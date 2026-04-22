using JCC.LifeCycle;
using UnityEngine;

namespace JCC.Utils.Art
{
    public interface IColorChanger : ILifeCycle
    {
        public Color GetColor();
        public void SetColor(Color color);
    }
}