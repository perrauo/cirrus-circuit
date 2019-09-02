using UnityEngine;
using System.Collections;

namespace Cirrus.Gembalaya.Objects
{
    [System.Serializable]
    public class StateMachine : FSM.Lightweight.Machine
    {
        [System.Serializable]
        public enum State
        {
            Entering,
            Falling,
            Idle,
            Moving
        }

        [SerializeField]
        private State _state = State.Idle;

        [SerializeField]
        private BaseObject _object;

        public override void Awake()
        {
            TryChangeState(State.Idle);
        }

        public override void FixedUpdate()
        {
            switch ((object)_state)
            {
                case State.Entering:
                case State.Falling:
                case State.Idle:
                case State.Moving:

                    _object.transform.position = Vector3.Lerp(_object.transform.position, _object._targetPosition, _object._stepSpeed);

                    float scale = Mathf.Lerp(_object.transform.localScale.x, _object._targetScale, _object._scaleSpeed);
                    _object.transform.localScale = new Vector3(scale, scale, scale);

                    break;
            }


        }

        public override void Update()
        {
            switch ((object)_state)
            {
                case State.Entering:
                case State.Falling:
                case State.Idle:
                case State.Moving:

                    if (Utils.Vectors.CloseEnough(transform.position, _object._targetPosition))
                    {
                        // If the destination can coexist with incoming object once arrived we return true
                        if (
                            _object._destination == null || 
                            !_object._destination.Accept(_object))
                        {
                            TryChangeState(State.Idle);
                        }
                    }

                    break;
            }
        }


        protected override bool VerifyTransition<T>(T state, params object[] args)
        {
            State target = (State)(object)state;

            switch (_state)
            {
                case State.Entering:

                    switch (target)
                    {
                        case State.Entering:
                        case State.Falling:
                        case State.Idle:
                        case State.Moving:
                            return true;
                    }
                    break;

                case State.Falling:
                    switch (target)
                    {
                        case State.Entering:
                        case State.Falling:
                        case State.Idle:
                        case State.Moving:
                            return true;
                    }
                    break;

                case State.Idle:
                    switch (target)
                    {
                        case State.Entering:
                        case State.Falling:
                        case State.Idle:
                        case State.Moving:
                            return true;
                    }
                    break;

                case State.Moving:
                    switch (target)
                    {
                        case State.Entering:
                        case State.Falling:
                        case State.Idle:
                        //case State.Moving:
                            return true;
                    }
                    break;
            }

            return false;
        }

        protected override bool TryFinishChangeState<T>(T state, params object[] args)
        {
            State target = (State)(object)state;

            switch (target)
            {
                case State.Entering:

                    _state = target;
                    _object._targetScale = 0;
                    return true;

                case State.Falling:

                    _state = target;
                    _object._targetPosition += Vector3.up * -_object._fallDistance;
                    _object._stepSpeed = _object._fallSpeed;
                    return true;

                case State.Idle:

                    _state = target;
                    _object._collider.enabled = true;
                    return true;

                case State.Moving:

                    Vector3 step = (Vector3)args[0];
                    BaseObject incoming = (BaseObject)args[1];

                    if (Physics.Raycast(
                        _object._targetPosition + Vector3.up / 2,
                        step,
                        out RaycastHit hit,
                        Levels.Level.CubeSize / 2))
                    {
                        _object._destination = hit.collider.GetComponent<BaseObject>();

                        if (_object._destination != null)
                        {
                            if (_object._destination.TryMove(step, _object))
                            {
                                _state = target;
                                _object._collider.enabled = false;
                                _object._targetPosition += step;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        _state = target;
                        _object._destination = null;
                        _object._collider.enabled = false;
                        _object._targetPosition += step;
                        return true;
                    }

                    break;

                default:
                    return false;
            }

            return false;
        }

    }
}