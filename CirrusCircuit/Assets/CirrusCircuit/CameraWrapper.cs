using UnityEngine;
using System.Collections;


namespace Cirrus.Circuit
{
    public class CameraWrapper : MonoBehaviour
    {
        [SerializeField]
        public Camera Camera;

        public void OnValidate()
        {
            if (Camera == null)
                Camera = GetComponent<Camera>();

            if (Camera == null)
                Camera = FindObjectOfType<Camera>();
        }


        public enum State
        {
            //Game
        }

        public void TryChangeState(State state)
        {

        }
    }
}
