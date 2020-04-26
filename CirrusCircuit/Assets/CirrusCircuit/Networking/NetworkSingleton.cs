//using UnityEngine;
//using System.Collections;

//namespace Cirrus.Circuit.Networking
//{
//    public class NetworkSingleton : Mirror.NetworkBehaviour { }


//    public class BaseNetworkSingleton<T> : NetworkSingleton where T : NetworkSingleton
//    {
//        public virtual void Awake()
//        {
//            //OnAwakeHandler?.Invoke();
//            //SceneManager.sceneLoaded += OnSceneLoaded;
//        }

//        public virtual void OnDestroy()
//        {
//            //OnDestroyHandler?.Invoke();
//        }

//        //public virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//        //{
//        //    //OnSceneLoadedHandler?.Invoke(scene, mode);
//        //}

//        public virtual void Start()
//        {
//            //OnStartHandler?.Invoke();
//        }

//        public virtual void OnEnable()
//        {
//            //OnEnableHandler?.Invoke();
//        }

//        public virtual void OnDisable()
//        {
//            //OnDisableHandler?.Invoke();
//        }


//        public virtual void OnValidate()
//        {

//        }


//        protected static T _instance;

//        public void Persist()
//        {
//            if (_instance != null)
//            {
//                DestroyImmediate(gameObject);
//                return;
//            }

//            _instance = Instance;
//            transform.SetParent(null);
//            DontDestroyOnLoad(gameObject);
//        }

//        public static T Instance
//        {
//            get
//            {
//                if (_instance == null)
//                {
//                    _instance = FindObjectOfType<T>();
//                }

//                return _instance;
//            }
//        }
//    }

//    //public class PersistentSingleton<T> : BaseNetworkSingleton<T> where T : NetworkSingleton
//    //{
//    //    public override void Awake()
//    //    {
//    //        if (_instance != null)
//    //        {
//    //            DestroyImmediate(gameObject);
//    //            return;
//    //        }

//    //        DontDestroyOnLoad(gameObject);
//    //    }

//    //}
//}