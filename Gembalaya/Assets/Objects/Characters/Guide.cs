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

        private float _raycastDistance = 100f;

        // Show count from here
        // raycast ignore moveable 
        // goodnight
        public void Show(Vector3 step, int count=0)
        {
            if (
            Physics.Raycast(
                transform.position + Vector3.up,
                step,
                out RaycastHit hit,
                _raycastDistance,
                Game.Instance.Layers.Moveable
                ))
            {
                Debug.Log(hit.distance);
            }

            //for (int i = 0; i < count; i++)
            //{
            //    Instantiate(_squareTemplate, transform.position + step * i, Quaternion.identity, transform);
            //}

        }

    }
}
