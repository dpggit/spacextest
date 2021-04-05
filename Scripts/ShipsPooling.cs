using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceX
{
    public class ShipsPooling : MonoBehaviour
    {
        public GameObject ObjectToPool;
        public int PoolLength;
        public int ActiveUptoThisNumber;

        [HideInInspector]
        public GameObject[] ObjectPoolArray;

        private void Awake()
        {
            if (ObjectToPool != null) ObjectPoolArray = new GameObject[PoolLength];
        }

        void Start()
        {
            StartPool();
        }

        //Instantate the objects and set up to the limit i set 
        private void StartPool()
        {
            if (ObjectToPool != null)
            {
                for (int i = 0; i < PoolLength; i++)
                {
                    ObjectPoolArray[i] = Instantiate(ObjectToPool);
                    ObjectPoolArray[i].name = "Ship_" + i;
                    ObjectPoolArray[i].SetActive((i > ActiveUptoThisNumber - 1) ? false : true);
                }
            }
        }

        //Return the next available object and activate it
        public GameObject GetAvailableObjectInPool()
        {
            for (int i = 0; i < ObjectPoolArray.Length; i++)
            {
                if (i >= ObjectPoolArray.Length) i = 0;
                if (!ObjectPoolArray[i].activeInHierarchy)
                {
                    ObjectPoolArray[i].SetActive(true);
                    return ObjectPoolArray[i];
                }
            }
            return null;
        }
    }
}
