using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit
{
    public class Clock : BaseSingleton<Clock>
    {
        public Delegate OnUpdateHandler;

        public Delegate OnFixedUpdateHandler;

        public void Update()
        {
            OnUpdateHandler?.Invoke();
        }

        public void FixedUpdate()
        {
            OnFixedUpdateHandler?.Invoke();
        }
    }
}
