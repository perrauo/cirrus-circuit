using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit.World.Objects
{
    public class Guide : MonoBehaviour
    {
        [SerializeField]
        private GameObject _squareTemplate;

        [SerializeField]
        private SpriteRenderer _templateSpriteRenderer;

        // square pool
        private List<GameObject> _squares;

        
        private Collections.GameObjectPool _pool;

        private const int PoolSize = 10;
        
        private const float Alpha = 0.6f;

        private const float RaycastDistance = 100f;


        public void Awake()
        {
            _squares = new List<GameObject>();
            _pool = new Collections.GameObjectPool(_squareTemplate, PoolSize);
        }

        private Color _color;

        public Color Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
                _color.a = Alpha;
                _templateSpriteRenderer.color = _color;
            }
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
                RaycastDistance,
                // Collide everything except layer 8
                ~Layers.MoveableFlags
                ))
            {
                int count = Mathf.FloorToInt(hit.distance / World.Level.CellSize);

                foreach (var s in _squares) if (s) _pool.Release(s);

                _squares.Clear();

                for (int i = 0; i < count; i++)
                {
                    GameObject square = _pool.Get();
                    square.GetComponentInChildren<SpriteRenderer>().color = _color;
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
