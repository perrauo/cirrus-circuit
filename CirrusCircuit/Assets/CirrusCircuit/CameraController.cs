using UnityEngine;
using System.Collections;
using UnityEngine.PlayerLoop;
//using Boo.Lang;
using Cirrus.Circuit.World.Objects;
using Cirrus.Circuit.World;
using System.Linq;
using System.Collections.Generic;

namespace Cirrus.Circuit
{
    public enum CameraState
    { 
        Idle,
        Round,
        Podium
    }

    public class CameraController : BaseSingleton<CameraController>
    {
        [SerializeField]
        private Camera _camera;
        public Camera Camera => _camera;

        [SerializeField]
        private CameraState _state;

        [SerializeField]
        public List<BaseObject> _targets = new List<BaseObject>();

        private Vector3 _startPosition;

        private Quaternion _startRotation = Quaternion.identity;

        private Vector3 _targetPosition;

        private Quaternion _targetRotation = Quaternion.identity;

        private const float Speed = 5f;

        private const float RotationSpeed = 5f;

        [SerializeField]
        private Vector3 Offset = new Vector3(-5, 20, -5);

        public override void OnValidate()
        {
            base.OnValidate();

            if (_camera == null) _camera = GetComponent<Camera>();
            if (_camera == null) _camera = FindObjectOfType<Camera>();
        }

        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();

            _startPosition = Camera.transform.position;
            _startRotation = Camera.transform.rotation;


            Game.Instance.OnPodiumHandler += () => SetState(CameraState.Podium);
            Game.Instance.OnRoundHandler += () => SetState(CameraState.Round); //() => RoundSession.Instance.OnRoundStartHandler += OnRoundStart;  
            Game.Instance.OnFinalPodiumHandler += () => SetState(CameraState.Podium);
            Game.Instance.OnLevelSelectHandler += (x) => { if (x) SetState(CameraState.Idle); };
        }

        public virtual void Update()
        {
            switch (_state)
            {
                case CameraState.Idle:
                    _targetPosition = _startPosition;
                    _targetRotation = _startRotation;
                    break;

                case CameraState.Round:

                    Vector3 total =
                        _targets.Aggregate(Vector3.zero, (sum, next) => sum + next.Transform.position);

                    Vector3 avg = total / (_targets.Count() + 1);

                    // Create a postion the camera is aiming for based on the offset from the target.
                    _targetPosition = avg + Offset;
                    _targetRotation = Quaternion.LookRotation(avg - transform.position);

                    break;


                case CameraState.Podium:
                    _targetPosition = _startPosition;
                    _targetRotation = _startRotation;
                    break;
            }
        }

        public virtual void LateUpdate()
        {
            switch (_state)
            {
                case CameraState.Idle:
                    Camera.transform.position = Vector3.Lerp(transform.position, _targetPosition, Speed * Time.deltaTime);
                    Camera.transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, RotationSpeed * Time.deltaTime);
                    break;

                case CameraState.Round:


                    //// Smoothly interpolate between the camera's current position and it's target position.
                    Camera.transform.position = Vector3.Lerp(transform.position, _targetPosition, Speed * Time.deltaTime);
                    Camera.transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, RotationSpeed * Time.deltaTime);

                    break;


                case CameraState.Podium:
                    Camera.transform.position = Vector3.Lerp(transform.position, _targetPosition, Speed * Time.deltaTime);
                    Camera.transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, RotationSpeed * Time.deltaTime);
                    break;
            }
        }


        public void SetState(CameraState state)
        {
            switch (state)
            {
                case CameraState.Idle:
                    _targets.Clear();
                    break;

                case CameraState.Round:
                    _targets.AddRange(LevelSession.Instance.Characters);
                    break;


                case CameraState.Podium:
                    _targets.Clear();
                    break;
            }

            _state = state;
        }

    }
}
