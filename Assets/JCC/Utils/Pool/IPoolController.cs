using UnityEngine;

namespace JCC.Utils.Pool
{
    public interface IPoolController<TType> where TType : MonoBehaviour
    {
        public void SetPoolObject(GameObject initialPoolObject, int poolSize, bool expandPool);
        public TType GetPoolObject();
        public void ReturnToPool(TType newPoolObj);
        public int GetCurrentPoolSize();
    }
}