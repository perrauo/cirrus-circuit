using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit
{
    public delegate void OnPlatformFinished();

    public class Platform : MonoBehaviour
    {
        public OnPlatformFinished OnPlatformFinishedHandler;

        [SerializeField]
        private UnityEngine.UI.Text _text;

        private float _score;

        private float _maxScore;

        //[SerializeField]
        //public Controls.Controller _controller;

        //public Controls.Controller Controller
        //{
        //    get
        //    {
        //        return _controller;
        //    }

        //    set
        //    {
        //        _controller = value;
        //    }
        //}
        private Controls.PlayerSession _player;

        [SerializeField]
        public GameObject _characterAnchor;

        [SerializeField]
        public World.Objects.Characters.Character _character;

        public World.Objects.Characters.Character Character
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

        public void Awake()
        {
            _finishedTimer = new Timer(_growthTime, start: false, repeat: false);
            _finishedTimer.OnTimeLimitHandler += OnPlatformFinishedTimeOut;
        }

        public void Update()
        {
            _score = Mathf.Lerp(_score, _maxScore, _finishedTimer.Time / _growthTime);
            _text.text = _score.ToString("n1");
        }



        public void FixedUpdate()
        {
            _character.Transform.position = _characterAnchor.transform.position;        
        }

        public void OnTransitionToTimeOut()
        {
            _maxScore += _player.Score;
            _finishedTimer.Start();
            Grow();
        }

        public Platform Create(Vector3 position, Transform parent, Controls.PlayerSession controller)
        {
            var val = Instantiate(
                gameObject,
                position,
                Quaternion.identity,
                parent).GetComponent<Platform>();

            //val._maxScore = score;
            val._player = controller;
            return val;
        }

        public void Grow()
        {
            iTween.ScaleAdd(
                _visual.Parent.gameObject, 
                new Vector3(0, _player.Score * _growthFactor, 0), 
                _growthTime);

            iTween.MoveAdd(
                _visual.Parent.gameObject, 
                new Vector3(0, _player.Score * _growthFactor, 0), 
                _growthTime);

            iTween.MoveAdd(
                _characterAnchor.gameObject, 
                new Vector3(0, _player.Score * 2 * _growthFactor, 0), 
                _growthTime);
        }

        public void OnPlatformFinishedTimeOut()
        {
            OnPlatformFinishedHandler?.Invoke();
        }
    }
}