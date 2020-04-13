using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit
{
    public class Clock : MonoBehaviour
    {
        public delegate void OnTick();
        public OnTick OnTickedHandler;

        public static Clock Instance;

        public void Awake()
        {
            Instance = this;
        }

        public void FixedUpdate()
        {
            OnTickedHandler?.Invoke();//
        }


        // TODO: in order to move clock to cirrus.
        //public CreateTimer(float limit, bool start = true, bool repeat = false)
        //{

        //}

    }
}
