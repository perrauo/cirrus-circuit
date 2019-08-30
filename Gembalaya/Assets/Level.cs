using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cirrus.Gembalaya.Levels
{
    public class Level : MonoBehaviour
    {
        public static float CubeSize = 2f;

        public static Level Instance;
        public void Awake()
        {
  
            Instance = this;

 
        }

    }
}