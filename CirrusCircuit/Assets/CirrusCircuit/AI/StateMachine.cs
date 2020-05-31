using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using Cirrus.FSM;
using Cirrus.Circuit.World.Objects.Characters;

namespace Cirrus.Circuit.AI
{
    [Serializable]
    public enum State
    {
        Attack,
        Wander,
        WanderInPlace,
        Chase,
        //Angry
    }

    public abstract class Transition
    {
        public abstract int State { get; }

        public float Chance;

        public Transition(float chance = 0.5f)
        {
            Chance = chance;
        }
    }


    [Serializable]
    public abstract class CharacterState : Cirrus.FSM.State
    {

        protected IEnumerable<Transition> _transitions;

        //[SerializeField]
        //protected List<NesScripts.Controls.PathFind.Point> _path;

        protected Vector2Int _finalDestination;

        protected Vector3 _nextDestination;

        protected int _currentPathPositionIndex = 0;

        protected Timer _timer;

        public virtual Character Character => _character;

        private Character _character;

        protected virtual float Time => StateMachine.DefaultStateTime;

        public CharacterState(
            Character character,
            bool isStart,
            params Transition[] transitions
            ) :
            base(
                character,
                isStart)
        {
            _character = character;

            _transitions = transitions;

            _timer = new Timer(Time, start: false);

            _timer.OnTimeLimitHandler += OnTimeout;
        }

        public override void Enter(
            params object[] args)
        {
            _timer.Start(Time);
        }

        public override void Exit()
        {
            _timer.Stop();
        }

        public override void BeginUpdate() { }

        public override void EndUpdate() { }


        public override void OnMachineDestroyed()
        {
            _timer.OnTimeLimitHandler -= OnTimeout;
        }

        public virtual void OnTimeout()
        {
            DoRandomTransition();
        }

        public virtual void DoRandomTransition()
        {
            //if (_transitions.Count() == 0)
            //    return;

            //while (true)
            //{
            //    var transition = _transitions.Random();

            //    if (
            //        Numeric.Chance.CheckIsTrue(
            //            transition.Chance))
            //    {
            //        if (Character.FSM.TrySetState(transition.StateId))
            //            break;
            //    }
            //}
        }

        public void CalculatePath(Vector2Int dest)
        {

        }

        public virtual void OccupyPosition()
        {

        }

        public virtual bool OnCellChanged()
        {


            return false;
        }

        public virtual void OnArrivedToDestination()
        {
            //Character.FSM.TrySetState(StateId.Idle);
        }

        public void FollowPath()
        {

   
        }
    }

    


    public class StateMachine : BaseMachine3
    {
        public const float DefaultStateTime = 1;

        public override void Awake()
        {
            base.Awake();

            //Add(new Disabled(_character,
            //    false));
        }


        public override void Update()
        {
            base.Update();
        }

        public override void Start()
        {
            base.Start();
        }
    }

}