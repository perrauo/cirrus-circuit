using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Gembalaya.Objects.Characters
{
    public class Guide : MonoBehaviour
    {
        [SerializeField]
        private GameObject _squareTemplate;

        // square pool
        private List<GameObject> _squares;

        
        private Collections.GameObjectPool _pool;

        [SerializeField]
        private int _poolSize = 10;//.GameObjectPool _pool;

        private float _raycastDistance = 100f;


        public void Awake()
        {
            _squares = new List<GameObject>();
            _pool = new Collections.GameObjectPool(_squareTemplate, _poolSize);
        }

        // Show count from here
        // raycast ignore moveable 
        // goodnight
        public void Show(Vector3 step)
        {
            if (
            Physics.Raycast(
                transform.position + Vector3.up,
                step,
                out RaycastHit hit,
                _raycastDistance,
                // Collide everything except layer 8
                ~Game.Instance.Layers.MoveableFlags
                ))
            {
                Debug.Log(hit.distance);


                int count = Mathf.FloorToInt(hit.distance / Levels.Level.CubeSize);


                foreach (var s in _squares)
                {
                    if (s)
                    {
                        _pool.Release(s);
                    }
                }

                _squares.Clear();

                for (int i = 0; i < count; i++)
                {
                    GameObject square = _pool.Get();
                    square.transform.SetParent(transform);
                    square.transform.position = transform.position + step * i;
                    _squares.Add(square);
                }

            }

        }

        public GameObject Recycle()
        {
            return null;
        }



    }
}
