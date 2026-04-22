using System;
using System.Collections.Generic;
using UnityEngine;

namespace JCC.Utils.Pool 
{ 
    public class PoolControllerImpl<TType> : IPoolController<TType> where TType : MonoBehaviour
    {
        private bool _expandPool = false;
        private GameObject _initialPoolObject = null;
        private Transform _parentPool;
        private List<PoolObject> _poolObjects = null;
        private List<TType> _poolSpecificObjects = null;

        public int GetCurrentPoolSize() => _poolObjects.Count;

        public void SetPoolObject(GameObject initialPoolObject, int poolSize, bool expandPool) 
        {
            _expandPool = expandPool;
            _poolObjects = new List<PoolObject>();
            _poolSpecificObjects = new List<TType>();
            _initialPoolObject = initialPoolObject;

            if (poolSize <= 0) 
                throw new ArgumentException("poolSize must be greater than zero", "poolSize");
            if (initialPoolObject == null)
                throw new ArgumentException("initialPoolObject is missing", "initialPoolObject");
            if (initialPoolObject.GetComponent<TType>() == null)
                throw new ArgumentException("initialPoolObject doesnt have the " + nameof(TType), "initialPoolObject");

            _parentPool = new GameObject("Pool-" + _initialPoolObject.name).transform;
            for (int i = 0; i < poolSize; i++)
            {
                AddNewElementIntoThePool(_initialPoolObject);
            }
        }

        public TType GetPoolObject() 
        {
            int index = _poolObjects.FindIndex(x => x.IsAvailable);

            if (index == -1 && _expandPool) 
            {
                AddNewElementIntoThePool(_initialPoolObject);
                index = _poolObjects.Count - 1;
            }

            if (index == -1) 
                throw new NullReferenceException("PoolObject wasnt found");

            _poolObjects[index].ActivatePoolObject();
            return _poolSpecificObjects[index];
        }

        public void ReturnToPool(TType newPoolObj) 
        {
            if (newPoolObj == null)
                throw new ArgumentException("newPoolObj is Null", "newPoolObj");

            if (_poolSpecificObjects.Contains(newPoolObj)) 
            {
                int index = _poolSpecificObjects.IndexOf(newPoolObj);
                _poolObjects[index].ReturnPoolObject();
                _poolObjects[index].transform.SetParent(_parentPool);
            }
            else
                throw new ArgumentException("newPoolObj ", "newPoolObj");
        }

        public void SetCustomTransform(Transform customParent) 
        {
            _parentPool.SetParent(customParent);
        }

        private void AddNewElementIntoThePool(GameObject initialPoolObject) 
        {
            PoolObject newPoolObj = CreateNewPoolObject(initialPoolObject);
            newPoolObj.transform.SetParent(_parentPool);
            _poolObjects.Add(newPoolObj);
            _poolSpecificObjects.Add(newPoolObj.GetComponent<TType>());
        }

        private PoolObject CreateNewPoolObject(GameObject initialPoolObject) 
        {
            GameObject obj = GameObject.Instantiate(initialPoolObject);
            PoolObject newPoolObj = obj.AddComponent<PoolObject>();
            newPoolObj.CreatePoolObject();
            return newPoolObj;
        }
    }
}