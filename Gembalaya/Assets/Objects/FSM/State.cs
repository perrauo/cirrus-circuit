//using UnityEngine;
//using UnityEditor;
//using KinematicCharacterController;
//using System;
//using System.Linq;



//// Character state

//namespace Cirrus.Gembalaya.Objects.FSM
//{
//    [System.Serializable]
//    public enum Id
//    {
//        Falling,
//        Entering,
//        Idle,
//        Moving,
//        Finished
//    }

//    public abstract class Resource : Cirrus.FSM.Resource
//    {
//        override public int Id { get { return -1; } }
//    }

//    public abstract class State : Cirrus.FSM.State
//    {

//        public State(object[] context, Cirrus.FSM.Resource resource) : base(context, resource) { }
//        protected BaseObject Object { get { return (BaseObject)context[0]; } }

//        protected Vector3 _targetPosition;

//        protected float _targetScale = 1;

//        public override void Enter(params object[] args)
//        {
//            _targetPosition = Object.transform.position;
//        }

//        public override void Exit() { }
//        public override void BeginUpdate() { }
//        public override void EndUpdate()
//        {

//        }

//        public virtual bool TryEnter()//Vector3 step, BaseObject incoming = null)
//        {
//            return false;
//        }

//        public virtual bool TryMove(Vector3 step, BaseObject incoming = null)
//        {
//            if (Physics.Raycast(
//                _targetPosition + Vector3.up / 2,
//                step,
//                out RaycastHit hit,
//                Levels.Level.CubeSize / 2))
//            {
//                BaseObject obj = hit.collider.GetComponent<BaseObject>();

//                if (obj != null)
//                {
//                    if (obj.TryMove(step, Object))
//                    {
//                        Object.Collider.enabled = false;
//                        _targetPosition += step;
//                        return true;
//                    }
//                }
//            }
//            else
//            {
//                Object.Collider.enabled = false;
//                _targetPosition += step;
//                return true;
//            }

//            return false;
//        }

//        public override void BeginFixedUpdate()
//        {
//            Object.transform.position = Vector3.Lerp(Object.transform.position, _targetPosition, Object.StepSpeed);
//            float scale = Mathf.Lerp(Object.transform.localScale.x, _targetScale, Object.ScaleSpeeed);
//            Object.transform.localScale = new Vector3(scale, scale, scale);
//        }

//        public override void EndFixedUpdate()
//        {

//            //base.EndFixedUpdate();
//        }

//    }


//}
