using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit.World.Objects.Characters
{
    public class Placeholder : BaseObject
    {
        public override ObjectType Type => ObjectType.CharacterPlaceholder;

        [SerializeField]
        private CameraController _camera;

        //adjust this to change speed        
        private const float Speed = 5f;
        
        //adjust this to change how high it goes
        private const float Height = 0.5f;

        private const float HeightOffset = 0.5f;

        public override void OnValidate()
        {
            base.OnValidate();

            if (_camera == null)
                _camera = FindObjectOfType<CameraController>();
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
            float newY = Mathf.Sin(Time.time * Speed)*Height;
            //set the object's Y to the new calculated Y
            _visual.Parent.transform.position = new Vector3(pos.x, pos.y+ HeightOffset + newY, pos.z);
        }

        //Orient the camera after all movement is completed this frame to avoid jittering
        public void LateUpdate()
        {
            _visual.Parent.transform.LookAt(_visual.Parent.transform.position + _camera.transform.rotation * Vector3.forward,
                _camera.transform.rotation * Vector3.up);
        }

    }
}
