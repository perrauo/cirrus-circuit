using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace Cirrus.Gembalaya
{
    public class Layers
    {
        //public int 
        public int ObjectsFlags = 1 << LayerMask.NameToLayer("Objects");
        public int LayoutFlags = 1 << LayerMask.NameToLayer("Layout");
        public int Objects = LayerMask.NameToLayer("Objects");
        public int Layout = LayerMask.NameToLayer("Layout");
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
        public Controls.Player[] Players;



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
