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
        private UI.Announcement _announcement;

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

        private Timer _finalTimer;

        [SerializeField]
        private float _timeFinal = 3f;

        [SerializeField]
        private float _timeTransitionFrom = 2f;

        [SerializeField]
        public float _positionSpeed = 0.4f;


        private bool _isFinal = false;

        private int _platformFinishedCount = 0;

        public void OnValidate()
        {
            if (_announcement == null)
                _announcement = FindObjectOfType<UI.Announcement>();
        }

        public void Awake()
        {
            _timer = new Timer(_timeTransition, start: false, repeat: false);
            _finalTimer = new Timer(_timeFinal, start: false, repeat: false);

            _finalTimer.OnTimeLimitHandler += OnFinalTimeout;

            Game.Instance.OnPodiumHandler += OnPodium;
            Game.Instance.OnFinalPodiumHandler += OnFinalPodium;
        }

        public void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, TargetPosition, _positionSpeed);

            for(int i = 0; i < _characters.Count; i++) {
                _characters[i].Transform.transform.position =
                Vector3.Lerp(
                    _characters[i].Transform.transform.position,
                    _platforms[i]._characterAnchor.transform.position,
                    _timer.Time/_timeTransitionFrom);

                _characters[i].Transform.transform.rotation = Quaternion.Lerp(
                    _characters[i].Transform.transform.rotation, 
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
            _isFinal = false;
            _platformFinishedCount = 0;
            _timer.Start();
        }

        public void OnFinalPodium()
        {
            _isFinal = true;
            _platformFinishedCount = 0;
            _timer.Start();
        }

        public void Clear()
        {
            foreach (var p in _platforms)
            {
                if (p == null)
                    continue;

                _timer.OnTimeLimitHandler -= p.OnTransitionToTimeOut;
                Destroy(p.gameObject);
            }

            _platforms.Clear();
            _characters.Clear();
        }

        public void Add(Controls.Player ctrl, World.Objects.Characters.CharacterAsset characterResource)
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

            character.ColorId = ctrl.Id;
            character.Color = ctrl.Color;

        }

        public void OnFinalTimeout()
        {
            OnPodiumFinishedHandler?.Invoke();
        }

        public void OnPlatformFinished()
        {
            _platformFinishedCount++;
            if (_platformFinishedCount >= _platforms.Count)
            {
                if (_isFinal)
                {
                    Controls.Player second = null;
                    float secondMax = -99999999f;
                    Controls.Player winner = null;
                    float max = -99999999f;
                    foreach (Controls.Player ctrl in Game.Instance._localPlayers)
                    {
                        if (ctrl.Score > max)
                        {
                            if (second == null)
                            {
                                second = winner;
                                secondMax = max;
                            }

                            winner = ctrl;
                            max = ctrl.Score;

                        }
                        else if (ctrl.Score > secondMax)
                        {
                            second = ctrl;
                            secondMax = ctrl.Score;
                        }
                    }

                    if (winner != null)
                    {
                        if (Mathf.Approximately(max, secondMax))
                        {
                            _announcement.Message = "Tie.";
                        }
                        else
                        {
                            _announcement.Message = winner.Name + " wins!";
                        }
                    }

                    _finalTimer.Start();
                }
                else
                {

                    OnPodiumFinishedHandler?.Invoke();
                }
            }
        }


    }
}