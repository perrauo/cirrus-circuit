
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;
//using Cirrus.GemCircuit.Objects.Characters.Actions;
using UnityInput = UnityEngine.InputSystem;// .Input;
//using Cirrus.GemCircuit.Controls;


// Controls Navmesh Navigation


namespace Cirrus.GemCircuit.Objects.Characters.Controls
{
    public class AIController : MonoBehaviour
    {
        [SerializeField]
        private Agent _agent;

        [SerializeField]
        private Character _character;

               
        protected void Awake()
        {

        }

        public void Start()
        {
 
        }

        public void Update()
        {

        }


        public void Jump()
        {
            //State.Jump();
        }


 



    }
}