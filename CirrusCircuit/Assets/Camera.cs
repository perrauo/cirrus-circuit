using UnityEngine;
using System.Collections;


namespace Cirrus.Circuit
{
    public class Camera : MonoBehaviour
    {
        [SerializeField]
        public UnityEngine.Camera UnityCamera;

        public void OnValidate()
        {
            if (UnityCamera == null)
                UnityCamera = GetComponent<UnityEngine.Camera>();

            if (UnityCamera == null)
                UnityCamera = FindObjectOfType<UnityEngine.Camera>();
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
