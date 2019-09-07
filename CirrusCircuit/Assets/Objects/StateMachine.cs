using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit.Objects
{
    [System.Serializable]
    public class StateMachine : MonoBehaviour
    {
        [System.Serializable]
        public enum State
        {
            Entering,
            Falling,
            Idle,
            RampIdle,
            Moving,
            RampMoving
        }

        [SerializeField]
        private State _state = State.Idle;

        [SerializeField]
        private BaseObject _object;

        public void Awake()
        {
            TryChangeState(State.Idle);
        }

        public void FixedUpdate()
        {
            switch (_state)
            {
                case State.Entering:
                case State.Falling:
                case State.Idle:
                case State.RampIdle:
                case State.Moving:
                case State.RampMoving:

                    _object.Object.transform.position = Vector3.Lerp(_object.Object.transform.position, _object._targetPosition, _object._stepSpeed);

                    float scale = Mathf.Lerp(_object.Object.transform.localScale.x, _object._targetScale, _object._scaleSpeed);
                    _object.Object.transform.localScale = new Vector3(scale, scale, scale);

                    break;
            }


        }

        public void Update()
        {
            switch (_state)
            {
                case State.Entering:
                
                case State.Idle:
                case State.RampIdle:
                    return;

                case State.Falling:
                case State.Moving:
                case State.RampMoving:

                    if (Utils.Vectors.CloseEnough(_object.Object.transform.position, _object._targetPosition))
                    {
                        // If the destination can coexist with incoming object once arrived we return true
                        if (_object._destination == null)
                        {
                            RaycastHit hit;

                            if (Physics.Raycast(
                                _object._targetPosition,
                                Vector3.down,
                                out hit,
                                Levels.Level.BlockSize / 2))
                            {
                                TryChangeState(State.Idle);
                            }
                            else
                            {
                                // Raycast down to get distance
                                if (Physics.Raycast(
                                _object._targetPosition,
                                Vector3.down,
                                out hit,
                                10f))
                                {
                                    Debug.Log(_object._targetPosition);
                                    Debug.Log(hit.point);
                                    TryChangeState(State.Falling, hit.distance);
                                }
                                else
                                {
                                    // TODO
                                    // Fall to infinity and destroy
                                }
                            }
                        }
                        else
                        {
                            _object._destination.Accept(_object);
                        }
                    }

                    break;
            }
        }

        public bool TryChangeState(State transition, params object[] args)
        {
            if (TryTransition(transition, out State destination))
            {
                return TryFinishChangeState(destination, args);
            }

            return false;
        }

        protected bool TryTransition(State transition, out State destination, params object[] args)
        {
            switch (_state)
            {
                case State.Entering:

                    switch (transition)
                    {
                        case State.Entering:
                        case State.Falling:
                        case State.Idle:
                        case State.RampIdle:
                        case State.Moving:
                            destination = transition;
                            return true;
                    }
                    break;

                case State.Falling:
                    switch (transition)
                    {
                        case State.Entering:
                        case State.Falling:
                        case State.Idle:
                        case State.RampIdle:
                        case State.Moving:
                            destination = transition;
                            return true;
                    }
                    break;

                case State.Idle:
                    switch (transition)
                    {
                        case State.Entering:
                        case State.Falling:
                        case State.Idle:
                        case State.RampIdle:
                        case State.Moving:
                            destination = transition;
                            return true;
                    }
                    break;

                case State.RampIdle:
                    switch (transition)
                    {
                        case State.Moving:
                            //case State.Moving:
                            destination = State.RampMoving;
                            return true;

                        default:
                            destination = State.Idle;
                            return false;
                    }

                case State.Moving:
                    switch (transition)
                    {
                        case State.Entering:
                        case State.Falling:
                        case State.Idle:
                        case State.RampIdle:
                            //case State.Moving:
                            destination = transition;
                            return true;
                    }
                    break;

                case State.RampMoving:
                    switch (transition)
                    {
                        case State.Entering:
                        case State.Falling:
                        case State.Idle:
                        case State.RampIdle:
                            //case State.Moving:
                            destination = transition;
                            return true;
                    }
                    break;
            }

            destination = State.Idle;
            return false;
        }

        protected bool TryFinishChangeState(State target, params object[] args)
        {
            Vector3 step;
            BaseObject incoming;
            RaycastHit hit;

            switch (target)
            {
                case State.Entering:
                    _state = target;
                    _object._targetScale = 0;
                    return true;

                case State.Falling:

                    float distance = (float)args[0];
                    _state = target;
                    _object._targetPosition += Vector3.down * distance;
                    _object._targetPosition += Vector3.up * Levels.Level.BlockSize/2;
                    //_object._stepSpeed = _object._fallSpeed;
                    return true;

                case State.Idle:
                    _state = target;
                    _object._collider.enabled = true;
                    return true;

                case State.RampIdle:

                    _state = target;
                    _object._collider.enabled = false;
                    return true;

                case State.RampMoving:

                    step = (Vector3)args[0];
                    incoming = (BaseObject)args[1];

                    Vector3 offset = Vector3.zero;;
                    // Determine which direction to cast the ray

                    Ray ray;

                    // Same direction (Look up)
                    if (Utils.Vectors.CloseEnough(step.normalized, _object.Object.transform.forward))
                    {
                        ray = new Ray(_object._targetPosition + Vector3.up * Levels.Level.BlockSize, step);
                        offset += Vector3.up * Levels.Level.BlockSize/2;
                    }
                    // Opposing direction (look down)
                    else if (Utils.Vectors.CloseEnough(step.normalized, -_object.Object.transform.forward))
                    {
                        ray = new Ray(_object._targetPosition + Vector3.down*Levels.Level.BlockSize, step);
                        offset -= Vector3.up * Levels.Level.BlockSize/2;
                    }
                    // Perp direction (Look ahead)
                    else
                    {
                        ray = new Ray(_object._targetPosition, step);
                    }

                    if (Physics.Raycast(ray, out hit, Levels.Level.BlockSize))
                    {
                        _object._collider.enabled = true;
                        var destination = hit.collider.GetComponentInParent<BaseObject>();

                        if (destination != null)
                        {
                            if (destination.TryMove(step, _object))
                            {
                                destination._user = null;
                                _object._destination = null; // We pushed it the destination was moved
                                _state = target;
                                _object._targetPosition += step;
                                _object._targetPosition += offset;
                                return true;
                            }
                            else if (destination.TryEnter(step, _object))
                            {
                                if (destination._user)
                                {
                                    if (destination._user.TryMove(step, _object))
                                    {
                                        destination._user = _object; // We pushed it the destination was moved
                                        _object._destination = destination;
                                        _state = target;
                                        _object._targetPosition += step;
                                        _object._targetPosition += offset;
                                        return true;
                                    }
                                }
                                else
                                {
                                    _state = target;
                                    destination._user = _object;
                                    _object._destination = destination;
                                    _object._targetPosition += step;
                                    _object._targetPosition += offset;
                                    return true;
                                }
                            }
                        }
                    }
                    else
                    {
                        _state = target;
                        if (_object._destination)
                            _object._destination._user = null;

                        _object._destination = null;
                        _object._targetPosition += step;
                        _object._targetPosition += offset;
                        return true;
                    }
                    
                    break;

                case State.Moving:

                    step = (Vector3)args[0];
                    incoming = (BaseObject)args[1];

                    // Raycast front
                    if (Physics.Raycast(
                        _object._targetPosition,
                        step,
                        out hit,
                        Levels.Level.BlockSize))
                    {
                        _object._collider.enabled = true;
                        var destination = hit.collider.GetComponentInParent<BaseObject>();

                        if (destination != null)
                        {
                            if (destination.TryMove(step, _object))
                            {
                                destination._user = null;
                                _object._destination = null; // We pushed it the destination was moved
                                _state = target;
                                _object._targetPosition += step;
                                return true;
                            }
                            else if (destination.TryEnter(step, _object))
                            {
                                if (destination._user)
                                {
                                    if (destination._user.TryMove(step, _object))
                                    {
                                        destination._user = _object; // We pushed it the destination was moved
                                        _object._destination = destination;
                                        _state = target;
                                        _object._targetPosition += step;
                                        return true;
                                    }
                                }
                                else
                                {
                                    _state = target;
                                    destination._user = _object;
                                    _object._destination = destination;
                                    _object._targetPosition += step;
                                    return true;
                                }
                            }
                        }
                    }
                    else
                    {
                        _state = target;
                        if (_object._destination)
                            _object._destination._user = null;

                        _object._destination = null;
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