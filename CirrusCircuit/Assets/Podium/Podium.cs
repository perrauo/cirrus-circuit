using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit
{
    public delegate  void OnPodiumFinished();


    public class Podium : MonoBehaviour
    {
        public OnPodiumFinished OnPodiumFinishedHandler;

        [SerializeField]
        private Platform _platformTemplate;

        [SerializeField]
        private float _platformOffset = 2f;

        [SerializeField]
        private GameObject _platformsParent;

        [SerializeField]
        public Vector3 TargetPosition = Vector3.zero;

        [SerializeField]
        private List<Platform> _platforms;

        [SerializeField]
        private List<World.Objects.Characters.Character> _characters;

        private Timer _timer;

        [SerializeField]
        private float _timeTransitionTo = 2f;

        [SerializeField]
        private float _timeTransitionFrom = 2f;

        [SerializeField]
        public float _positionSpeed = 0.4f;

        bool _init = false;

        private int _platformFinishedCount = 0;


        public void OnEnable()
        {
            if (!_init)
            {
                _init = true;
                _timer = new Timer(_timeTransitionTo, start: false, repeat: false);
            }
        }

        public void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, TargetPosition, _positionSpeed);

            for(int i = 0; i < _characters.Count; i++) {
                _characters[i].Object.transform.position =
                Vector3.Lerp(
                    _characters[i].Object.transform.position,
                    _platforms[i]._characterAnchor.transform.position,
                    _timer.Time/_timeTransitionFrom);

                _characters[i].Object.transform.rotation = Quaternion.Lerp(
                    _characters[i].Object.transform.rotation, 
                    _platforms[i]._visual.Parent.transform.rotation,
                    _timer.Time / _timeTransitionFrom);
            }
        }

        public bool IsEmpty {

            get
            {
                return _platforms.Count == 0;
            }
        }

        public void OnRound(Round round)
        {
            round.OnRoundEndHandler += OnRoundEnd;
        }

        public void Clear()
        {
            foreach (var p in _platforms)
            {
                if (p == null)
                    continue;

                Destroy(p.gameObject);
            }

            _platforms.Clear();

            _characters.Clear();
        }

        public void Add(Controls.Controller ctrl, World.Objects.Characters.Character character)
        {
            Platform platform = Instantiate(
                _platformTemplate.gameObject,
                _platformsParent.transform.position + Vector3.right * _platforms.Count * _platformOffset,
                Quaternion.identity,
                _platformsParent.transform).GetComponent<Platform>();

            _characters.Add(character);

            platform.Character = character;
            platform.Controller = ctrl;
            _platforms.Add(platform);

            _timer.OnTimeLimitHandler += platform.OnTransitionToTimeOut;
            platform.OnPlatformFinishedHandler += OnPlatformFinished;
        }

        public void OnRoundEnd()
        {
            _platformFinishedCount = 0;
            _timer.Start();
        }


        public void OnPlatformFinished()
        {
            _platformFinishedCount++;
            if (_platformFinishedCount >= _platforms.Count)
            {
                OnPodiumFinishedHandler?.Invoke();
            }
        }
    }
}