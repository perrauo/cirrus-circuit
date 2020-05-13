using UnityEngine;
using System.Collections;


namespace Cirrus.Circuit
{
    public class CameraManager : BaseSingleton<CameraManager>
    {
        [SerializeField]
        private Camera _camera;
        public Camera Camera => _camera;

        public override void OnValidate()
        {
            base.OnValidate();

            if (_camera == null) _camera = GetComponent<Camera>();
            if (_camera == null) _camera = FindObjectOfType<Camera>();
        }


        public enum State
        {
            //Game
        }

        public void TrySetState(State state)
        {

        }
    }
}
