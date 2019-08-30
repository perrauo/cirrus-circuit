
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