
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;
//using Cirrus.Gembalaya.Objects.Characters.Actions;
using UnityInput = UnityEngine.InputSystem;// .Input;
//using Cirrus.Gembalaya.Controls;


// Controls Navmesh Navigation


namespace Cirrus.Gembalaya.Objects.Characters.Controls
{
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private Agent _agent;

        [SerializeField]
        private Character _character;

        [SerializeField]
        public Cirrus.FSM.Machine FSM;


        public virtual Vector2 AxesLeft
        {
            get
            {
                return _character.Axes.Left;
            }

            set {
                State.AxesLeft = value;
            }
        }

        public virtual Vector2 AxesRight
        {
            get
            {
                return _character.Axes.Right;
            }

            set
            {
                State.AxesRight = value;
            }
        }


        public FSM.State State
        {   get
            {
                return (FSM.State)FSM.Top;
            }
        }
        
        protected void Awake()
        {
            FSM.SetContext(this, 1);
            FSM.SetContext(_character, 2);
        }

        public void Start()
        {
            FSM.Start();
        }

        public void Update()
        {
            FSM.DoUpdate();
        }


        public void Jump()
        {
            State.Jump();
        }


 



    }
}