using System.Collections.Generic;
using System;

namespace JCC.Utils.Random
{
    public class RandomUnity : IRandom
    {
        public RandomUnity() 
        {
            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        }

        public int GetRandomIndexInList<TType>(List<TType> list) 
        {
            if(list == null)
                throw new ArgumentNullException("The list is null");

            return list.Count == 0 ? -1 : UnityEngine.Random.Range(0, list.Count);
        }

        public int GetRandomIntBetween(int minInclusive, int maxExclusive) 
        {
            if (minInclusive >= maxExclusive)
                throw new ArgumentException("minInclusive should be less than maxExclusive");

            return UnityEngine.Random.Range(minInclusive, maxExclusive);
        }

        public float GetRandomFloatBetween(float minInclusive, float maxInclusive)
        {
            if (minInclusive > maxInclusive)
                throw new ArgumentException("minInclusive should be less or equal than maxInclusive");

            return UnityEngine.Random.Range(minInclusive, maxInclusive);
        }
    }
}