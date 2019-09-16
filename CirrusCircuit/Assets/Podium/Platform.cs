using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit
{
    public delegate void OnPlatformFinished();

    public class Platform : MonoBehaviour
    {
        public OnPlatformFinished OnPlatformFinishedHandler;

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
        public World.Objects.Character _character;

        public World.Objects.Character Character
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

        private Timer _finishedTimer = null;


        private bool _init = false;

        public void OnEnable()
        {
            if (!_init)
            {
                _finishedTimer = new Timer(_growthTime, start: false, repeat: false);
                _finishedTimer.OnTimeLimitHandler += OnPlatformFinishedTimeOut;
            }            
        }


        public float Score
        {
            get
            {
                return _score;
            }

            set
            {
                _score = value;
                _finishedTimer.Start();
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


        public void OnPlatformFinishedTimeOut()
        {
            OnPlatformFinishedHandler?.Invoke();
        }
    }
}