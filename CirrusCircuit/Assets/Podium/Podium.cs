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
        private Game _game;

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
        private float _timeTransition = 2f;

        [SerializeField]
        private float _timeTransitionFrom = 2f;

        [SerializeField]
        public float _positionSpeed = 0.4f;

        private int _platformFinishedCount = 0;

        public void OnValidate()
        {
            if (_game == null)
                _game = FindObjectOfType<Game>();
        }

        public void Awake()
        {
            _timer = new Timer(_timeTransition, start: false, repeat: false);
            _game.OnPodiumHandler += OnPodium;
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
            //round.OnRoundEndHandler += OnRoundEnd;
        }

        public void OnPodium()
        {
            _platformFinishedCount = 0;
            _timer.Start();
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

        public void Add(Controls.Controller ctrl, World.Objects.Characters.Resource characterResource)
        {
            Platform platform = _platformTemplate.Create(
                _platformsParent.transform.position + Vector3.right * _platforms.Count * _platformOffset,
                _platformsParent.transform,
                ctrl);
            _timer.OnTimeLimitHandler += platform.OnTransitionToTimeOut;
            _platforms.Add(platform);
            platform.OnPlatformFinishedHandler += OnPlatformFinished;

            World.Objects.Characters.Character character = 
                characterResource.Create(platform._characterAnchor.transform.position, 
                platform._characterAnchor.transform);
            _characters.Add(character);
            character.transform.rotation = platform._visual.Parent.transform.rotation;
            platform.Character = character;
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