using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceX
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
