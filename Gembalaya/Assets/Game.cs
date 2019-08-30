using Cirrus.Gembalaya.Objects.Characters.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace Cirrus.Gembalaya
{
    public class Layers
    {
        //public int 

        public int Moveable = LayerMask.NameToLayer("Moveable");
        public int Solid = LayerMask.NameToLayer("Solid");
    }

    public class Game : MonoBehaviour
    {
        public static Game Instance;

        [SerializeField]
        private Clock _clock;

        public Clock Clock
        {
            get
            {
                return _clock;
            }
        }


        [SerializeField]
        public PlayerController[] Players;



        public Layers Layers;


        [RuntimeInitializeOnLoadMethod] // Will exec, even if no GObjs
        static void OnRuntimeMethodLoad()
        {

        }

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Layers = new Layers();
            Instance = this;
            DontDestroyOnLoad(this.gameObject);             
        }

        public void Start()
        {

            //Player.Enable();         
            
        }


        public void OnValidate()
        {

        }



    }
}
