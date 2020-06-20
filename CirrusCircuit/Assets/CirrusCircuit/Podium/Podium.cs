using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cirrus.Circuit.Networking;

using Mirror;
using Cirrus.Circuit.World.Objects.Characters;
using Cirrus.Circuit.UI;
using Cirrus.Circuit.Controls;

namespace Cirrus.Circuit
{
    public delegate  void OnPodiumFinished();


    public class Podium : BaseSingleton<Podium>
    {
        public OnPodiumFinished OnPodiumFinishedHandler;

        [SerializeField]
        private Platform _platformTemplate;

        private const float PlatformOffset = 2f;

        [SerializeField]
        private GameObject _platformsParent;

        [SerializeField]
        public Vector3 TargetPosition = Vector3.zero;

        [SerializeField]
        private List<Platform> _platforms;

        [SerializeField]
        private List<Character> _characters;

        private Timer _timer;
        
        private const float TimeTransition = 2f;

        private Timer _finalTimer;
        
        private const float TimeFinal = 3f;
        
        private const float TimeTransitionFrom = 2f;

        public const float PositionSpeed = 0.4f;

        public bool IsEmpty => _platforms.Count == 0;

        private bool _isFinal = false;

        private int _platformFinishedCount = 0;

        public override void OnValidate()
        {
            base.OnValidate();

           
        }

        public override void Awake()
        {
            base.Awake();

            _timer = new Timer(TimeTransition, start: false, repeat: false);
            _finalTimer = new Timer(TimeFinal, start: false, repeat: false);

            _finalTimer.OnTimeLimitHandler += () => OnPodiumFinishedHandler?.Invoke();

            GameSession.OnStartClientStaticHandler += OnClientStarted;
            Game.Instance.OnPodiumHandler += OnPodium;
            Game.Instance.OnFinalPodiumHandler += OnFinalPodium;
            OnPodiumFinishedHandler += Game.Instance.OnPodiumFinished;
        }

        public void OnClientStarted(bool enable)
        {

        }

        public void FixedUpdate()
        {
            transform.position = Vector3.Lerp(
                transform.position, 
                TargetPosition, 
                PositionSpeed);

            for(int i = 0; i < _characters.Count; i++) 
            {
                _characters[i].Transform.position =
                Vector3.Lerp(
                    _characters[i].Transform.position,
                    _platforms[i]._characterAnchor.transform.position,
                    _timer.Time/TimeTransitionFrom);

                _characters[i].Transform.rotation = Quaternion.Lerp(
                    _characters[i].Transform.rotation, 
                    _platforms[i]._visual.Parent.transform.rotation,
                    _timer.Time / TimeTransitionFrom);
            }
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
                if (p == null) continue;

                _timer.OnTimeLimitHandler -= p.OnTransitionToTimeOut;
                Destroy(p.gameObject);
            }

            _platforms.Clear();
            _characters.Clear();
        }

        public void Add(
            PlayerSession player, 
            CharacterAsset characterResource)
        {
            Platform platform = _platformTemplate.Create(
                _platformsParent.transform.position + Vector3.right * _platforms.Count * PlatformOffset,
                _platformsParent.transform,
                player);
            platform.OnPlatformFinishedHandler += OnPlatformFinished;
            _platforms.Add(platform);
            _timer.OnTimeLimitHandler += platform.OnTransitionToTimeOut;

            Character character = 
                characterResource.Create(
                    platform._characterAnchor.transform.position, 
                    platform._characterAnchor.transform);
            
            _characters.Add(character);
            character.transform.rotation = platform._visual.Parent.transform.rotation;
            platform.Character = character;

            character.ColorID = player.ServerId;
            character.Color = player.Color;
        }

        public void OnPlatformFinished()
        {
            _platformFinishedCount++;
            if (
                _platformFinishedCount >= 
                _platforms.Count)
            {
                if (_isFinal)
                {
                    PlayerSession second = null;
                    float secondMax = -99999999f;
                    PlayerSession winner = null;
                    float max = -99999999f;
                    foreach (
                        PlayerSession player in 
                        GameSession.Instance.Players)
                    {
                        if (player.Score > max)
                        {
                            if (second == null)
                            {
                                second = winner;
                                secondMax = max;
                            }

                            winner = player;
                            max = player.Score;

                        }
                        else if (player.Score > secondMax)
                        {
                            second = player;
                            secondMax = player.Score;
                        }
                    }

                    if (winner != null)
                    {
                        Announcement.Instance.Message = 
                            Mathf.Approximately(max, secondMax) ? 
                            "Tie." : 
                            winner.Name + " wins!";
                    }

                    _finalTimer.Start();
                }
                else OnPodiumFinishedHandler?.Invoke();
            }
        }


    }
}