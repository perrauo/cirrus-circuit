using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Threading;

namespace Cirrus.FSM
{
    /// <summary>
    /// TODO Handle events do not force everything to appear in the loop
    /// Make it so that some states respond to events
    /// </summary>

    [System.Serializable]
    public class Machine
    {
        [SerializeField]
        private GameObject _stateLabel;

        //Must be set manually
        [SerializeField]
        private int _contextSize = 5;
        public object[] Context = null;


        Mutex _mutex;

        public void SetContext(object context, int idx)
        {
            if(Context == null) this.Context = new object[_contextSize];
            Context[idx] = context;
        }

        public void Start()
        {
            _dictionary = new Dictionary<int, State>();
            _mutex = new Mutex(false);

            _current = Populate();

            // sets the first in the array as active
            if (_current != null)
            {
                TrySetState(_current.Id);
            }
        }

        public State _current;

       [SerializeField]
        public State Top
        {
            get { return _current; } }

        [SerializeField]
        public Resource[] states;

        private Dictionary<int, State> _dictionary;

        private bool _enabled = true;

        public void Disable()
        {
            _enabled = false;
        }

        public void Enable()
        {
            _enabled = true;
        }


        /// <summary>
        /// populates the dictionary and returns the first state
        /// </summary>
        /// <returns></returns>
        private State Populate()
        {
            _dictionary.Add((int)Cirrus.FSM.DefaultState.Idle, new Idle.State());
            State first = null;
            foreach (Resource res in states)
            {
                if (res != null)
                {
                    if (_dictionary.ContainsKey(res.Id))
                        continue;

                    State state = CreateState(res);
                    _dictionary.Add(res.Id, state);

                    if(first == null)
                        first = state;
                }

            }

            return first;

        }

        public virtual State CreateState(Resource resource)
        {
            return resource.Create(Context);
        }


        public string StateName = "";


        public void DoUpdate()
        {
            if (_stateLabel != null)
            {
                _stateLabel.name = Top.resource.name;
            }

            if (!_enabled)
                return;

            if (Top != null)
            {
                _mutex.WaitOne();
                Top.BeginTick();
                Top.EndTick();
                _mutex.ReleaseMutex();
            }
        }

        public void OnDrawGizmos()
        {
            if (!_enabled)
                return;


            if (Top != null)
            {
                Top.OnDrawGizmos();
            }
        }


        public void DrawGizmosIcon(Vector3 pos)
        {
            if (Top != null)
            {
                //Gizmos.DrawIcon(pos, Top.ToString(), true);
                //Handles.Label(pos, Top.ToString());
                //Utils.TextGizmo.Draw(pos, Top.ToString());

            }
        }

        public bool TrySetState<T>(T state, params object[] args)
        {
            return TrySetState((int)(object)state, args);
        }

        public bool TrySetState(int state, params object[] args)
        {
            if(_dictionary.TryGetValue(state, out State res))
            {
                _current?.Exit();

                if (_current != null && _current.Id == res.Id)
                    res.Reenter(args);
                else
                {
                    _mutex.WaitOne();
                    _current = res;
                    _mutex.ReleaseMutex();

                    res.Enter(args);
                }

                return true;
            }

            return false;
        }

        //public bool TryTransition<T>(T transition, params object[] args)
        //{
        //    if (Top == null)
        //        return false;

        //    int transId = (int)(object)transition;

        //    foreach (var trans in Top.Transitions)
        //    {
        //        if (trans == null)
        //            continue;

        //        if ((trans.Symbols & transId) != 0)
        //        {
        //            if (TrySetState(trans.Destination))
        //                return true;
        //        }
        //    }

        //    return false;
        //}
    }

}
