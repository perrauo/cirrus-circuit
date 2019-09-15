using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit.UI
{
    public class WorldScreenUI : MonoBehaviour
    {
        [SerializeField]
        private Transform _anchor;

        private Vector3 _target;

        private Vector3 _candidate;

        [SerializeField]
        private Vector3 _offset = Vector3.zero;


        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private float _smooth = 0.9f;


        [SerializeField]
        private RectTransform rect;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        void LateUpdate()
        {
            _camera.ResetWorldToCameraMatrix(); // Force camera matrix to be updated

            _candidate.x = Mathf.Round(_anchor.position.x * 100f) / 100f;
            _candidate.y = Mathf.Round(_anchor.position.y * 100f) / 100f;
            _candidate.z = Mathf.Round(_anchor.position.z * 100f) / 100f;
            _candidate = Camera.main.WorldToScreenPoint(_candidate);
            _candidate = Utils.Vectors.Round(_candidate); // Pixel snapping
            _candidate.z = 0;
            rect.position = _candidate + _offset;
        }


        public void OnValidate()
        {
            if(rect == null)
                rect = GetComponent<RectTransform>();

            if (_camera == null)
                _camera = FindObjectOfType<Camera>();

            if (_anchor != null)
            {

                _candidate.x = Mathf.Round(_anchor.position.x * 100f) / 100f;
                _candidate.y = Mathf.Round(_anchor.position.y * 100f) / 100f;
                _candidate.z = Mathf.Round(_anchor.position.z * 100f) / 100f;
                _candidate = Camera.main.WorldToScreenPoint(_candidate);
                _candidate = Utils.Vectors.Round(_candidate); // Pixel snapping
                _candidate.z = 0;
                rect.position = _candidate + _offset;
            }

        }
    }
}



