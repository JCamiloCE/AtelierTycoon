using Emc2.Scripts.Enums;
using JCC.Debug;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Emc2.Scripts.Camera
{
    [CreateAssetMenu(fileName = "CameraDatabase", menuName = "Atelier/Camera/Camera Database")]
    public class CameraViewDataScriptable : ScriptableObject
    {
        [SerializeField] private List<ViewEntry> _entries;

        public IReadOnlyList<EAtelierView> ViewOrder
        {
            get
            {
                List<EAtelierView> list = new List<EAtelierView>(_entries.Count);
                for (int i = 0; i < _entries.Count; i++)
                {
                    list.Add(_entries[i].view);
                } 
                return list;
            }
        }

        public ViewEntry GetDataByView(EAtelierView view)
        {
            ViewEntry entry = _entries.Find(e => e.view == view);
            if (entry == null)
            { 
                DebugManager.LogError($"[CameraViewData.GetDataByView] the {view} didn't found");
            }
            return entry;
        }

        [Serializable]
        public class ViewEntry
        {
            public EAtelierView view;

            [Header("Default State")]
            public Vector2 defaultPosition;
            public float   defaultZoom = 3f;

            [Header("Zoom")]
            public float minOrthographicSize = 1.5f;
            public float maxOrthographicSize = 4f;

            [Header("Bounds (World Space)")]
            public Vector2 minBounds;
            public Vector2 maxBounds;
        }
    }
}
