using UnityEngine;
using System.Collections;
//using Cirrus.DH.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Cirrus.GemCircuit.Cameras
{
    public enum State
    {
        Follow,
        Focus
    }


    public class CameraFollow : MonoBehaviour
    {
        private State _state;

        public Camera Camera;

        //public BaseObject _target;            // The position that that camera will be following.

        //public List<BaseObject> _targets;            // The position that that camera will be following.

        public float _smoothing = 5f;        // The speed with which the camera will be following.

        [SerializeField]
        private Vector3 _offset;                     // The initial offset from the target.


        [SerializeField]
        private float _rotationSpeed = 5;

        private void Awake()
        {
            //Levels.Room.OnRoomLoadedHandler += OnRoomLoaded;
        }


        void Start ()
        {
            //if (_target == null)
                return;

            // Calculate the initial offset.
            //offset = transform.position - target.position;
        }

        //public void SetFocusState(List<BaseObject> targets)
        //{
        //    _targets = targets;
        //    _state = State.Focus;
        //}

        //public void SetFollowState(BaseObject target)
        //{
        //    _target = target;
        //    _state = State.Follow;
        //}



        void LateUpdate()
        {

            Vector3 targetCamPos;
            Quaternion targetRotation;
            switch (_state)
            {

                case State.Follow:

                    //if (_target == null)
                    //    return;

                    //// Create a postion the camera is aiming for based on the offset from the target.
                    //targetCamPos = _target.transform.position + _offset;

                    ////// Smoothly interpolate between the camera's current position and it's target position.
                    //transform.position = Vector3.Lerp(transform.position, targetCamPos, _smoothing * UnityEngine.Time.deltaTime);

                    //targetRotation = Quaternion.LookRotation(_target.transform.position - transform.position);
                    //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * UnityEngine.Time.deltaTime);

                    break;

                case State.Focus:
                    //TODO: Weigh the player more or less not equal
                    //Vector3 total = _targets.Aggregate(Vector3.zero, (sum, next) => sum + next.transform.position);
                    //Vector3 avg = total / _targets.Count;

                    //if (_target == null)
                    //    return;

                    // Create a postion the camera is aiming for based on the offset from the target.
                    //targetCamPos = avg + _offset;

                    ////// Smoothly interpolate between the camera's current position and it's target position.
                    //transform.position = Vector3.Lerp(transform.position, targetCamPos, _smoothing * UnityEngine.Time.deltaTime);

                    //targetRotation = Quaternion.LookRotation(avg - transform.position);
                    //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * UnityEngine.Time.deltaTime);

                    break;

            }

        }






        //public void RegisterTarget(BaseObject baseObject)
        //{
        //    _target = baseObject;
        //}


        //public void OnRoomLoaded(Levels.Room room)
        //{
        //    //target = Game.Instance.Player.transform;
        //}

    }
}

