using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit.Objects
{
    public class Platform : MonoBehaviour
    {
        private float _score;

        [SerializeField]
        public Controls.Controller _controller;

        public Controls.Controller Controller
        {
            get
            {
                return _controller;
            }

            set
            {
                _controller = value;
            }
        }

        [SerializeField]
        public GameObject _characterAnchor;

        [SerializeField]
        public Characters.Character _character;

        public Characters.Character Character
        {
            get
            {
                return _character;
            }

            set
            {
                _character = value;

            }

        }

        [SerializeField]
        public Visual _visual;

        private float _growth;

        [SerializeField]
        private float _growthFactor = 0.2f;

        [SerializeField]
        private float _growthTime = 2f;


        public float Score
        {
            get
            {
                return _score;
            }

            set
            {
                _score = value;
                StartCoroutine(Grow());
            }

        }

        [SerializeField]
        public bool _transition = true;

        [SerializeField]
        private float _transitionSpeed = 0.05f;

        
        public void FixedUpdate()
        {
            if (_transition)
            {

            }
            else
            {
                _character.Object.transform.position = _characterAnchor.transform.position;
            }
        }

        public void OnTransitionToTimeOut()
        {
            _transition = false;
            Score = _controller.Score;
        }

        public IEnumerator Grow()
        {
            iTween.ScaleAdd(
                _visual.Parent.gameObject, 
                new Vector3(0, _score * _growthFactor, 0), 
                _growthTime);

            iTween.MoveAdd(
                _visual.Parent.gameObject, 
                new Vector3(0, _score * _growthFactor, 0), 
                _growthTime);

            iTween.MoveAdd(
                _characterAnchor.gameObject, 
                new Vector3(0, _score * 2 * _growthFactor, 0), 
                _growthTime);

            yield return null;
        }






    }
}