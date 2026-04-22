using System.Collections.Generic;
using UnityEngine;

namespace JCC.Utils.Pool
{
    public class PoolObject : MonoBehaviour
    {
        private List<IPoolResettable> _poolResettable = null;
        private bool _isAvailable = false;

        public bool IsAvailable => _isAvailable;

        internal void CreatePoolObject() 
        {
            GetAllResettablePoolObj();
            gameObject.SetActive(false);
            _isAvailable = true;
        }

        internal void ActivatePoolObject() 
        {
            gameObject.SetActive(true);
            _isAvailable = false;
        }

        internal void ReturnPoolObject() 
        {
            gameObject.SetActive(false);
            ResetResettables();
            _isAvailable = true;
        }

        private void GetAllResettablePoolObj() 
        {
            _poolResettable = new List<IPoolResettable>();
            var allComponents = GetComponentsInChildren<Component>(includeInactive:true);

            foreach (var component in allComponents)
            {
                if (component is IPoolResettable poolResettable)
                {
                    _poolResettable.Add(poolResettable);
                }
            }
        }

        private void ResetResettables() 
        {
            foreach (var reseteable in _poolResettable)
            {
                reseteable.ResetPoolObject();
            }
        }
    }
}