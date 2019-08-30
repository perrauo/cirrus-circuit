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


        public void Show(Vector3 step, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Instantiate(_squareTemplate, transform.position + step * i, Quaternion.identity, transform);
            }
        }

    }
}
