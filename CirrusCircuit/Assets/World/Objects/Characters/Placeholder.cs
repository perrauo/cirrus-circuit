using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit.World.Objects.Characters
{
    public class Placeholder : BaseObject
    {
        [SerializeField]
        private CameraWrapper _camera;

        //adjust this to change speed
        [SerializeField]
        private float speed = 5f;

        [SerializeField]
        //adjust this to change how high it goes
        private float height = 0.5f;

        [SerializeField]
        private float _offset = 0.5f;

        //private Vector3 _k

        public void OnValidate()
        {
            if (_camera == null)
                _camera = FindObjectOfType<CameraWrapper>();
        }

        public override void Start()
        {
            base.Start();

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            //get the objects current position and put it in a variable so we can access it later with less code
            Vector3 pos = transform.position;
            //calculate what the new Y position will be
            float newY = Mathf.Sin(Time.time * speed)*height;
            //set the object's Y to the new calculated Y
            _visual.Parent.transform.position = new Vector3(pos.x, pos.y+ _offset + newY, pos.z);
        }

        //Orient the camera after all movement is completed this frame to avoid jittering
        public void LateUpdate()
        {
            _visual.Parent.transform.LookAt(_visual.Parent.transform.position + _camera.transform.rotation * Vector3.forward,
                _camera.transform.rotation * Vector3.up);
        }

    }
}
