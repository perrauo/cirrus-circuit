using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Cirrus.Circuit.Controls;

namespace Cirrus.Circuit.UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField]
        private Game _game;

        [SerializeField]
        private PlayerDisplay[] _playerDisplays;

        private List<PlayerDisplay> _availablePlayerDisplays;


        [SerializeField]
        private UnityEngine.UI.Text _levelName;

        [SerializeField]
        private UnityEngine.UI.Text _previous;

        [SerializeField]
        private UnityEngine.UI.Text _next;

        [SerializeField]
        private Timer _timer;

        [SerializeField]
        private CountDown _countDown;


        [SerializeField]
        private float _selectPunchScale = 0.5f;

        [SerializeField]
        private float _selectPunchScaleTime = 1f;

        [SerializeField]
        private GameObject _playerDisplay;

        [SerializeField]
        private GameObject _levelSelectDisplay;

        [SerializeField]
        private GameObject _timesUp;

        private Circuit.Timer _timesUpTimer;

        [SerializeField]
        private float _timesUpTime = 2f;


        private Circuit.Round _round;

        [SerializeField]
        private Round _roundDisplay;


        private bool _init = false;

        public void OnEnable()
        {
            if (!_init)
            {
                _init = true;
                _availablePlayerDisplays = new List<PlayerDisplay>();

                _timesUpTimer = new Circuit.Timer(_timesUpTime, start: false, repeat: false);
                _timesUpTimer.OnTimeLimitHandler += OnTimesUpTimeOut;
            }
        }

        public IEnumerator PunchScale(bool previous)
        {
            iTween.Stop(_previous.gameObject);
            iTween.Stop(_next.gameObject);

            _previous.transform.localScale = new Vector3(1, 1, 1);
            _next.transform.localScale = new Vector3(1, 1, 1);

            yield return new WaitForSeconds(0.01f);

            if (previous)
            {

                iTween.PunchScale(
                    _previous.gameObject,
                    new Vector3(_selectPunchScale,
                    _selectPunchScale, 0),
                    _selectPunchScaleTime);
            }
            else
            {
                iTween.PunchScale(
                    _next.gameObject,
                    new Vector3(_selectPunchScale,
                    _selectPunchScale, 0),
                    _selectPunchScaleTime);
            }
        }

        public void OnWaiting()
        {
            _countDown.gameObject.SetActive(false);
            _timer.gameObject.SetActive(false);
            _levelSelectDisplay.SetActive(false);

            _playerDisplay.SetActive(true);

            _availablePlayerDisplays.Clear();
            _availablePlayerDisplays.AddRange(_playerDisplays);

            for (int i = 0; i < Game.Instance._selectedLevel.CharacterCount; i++)
            {
                _playerDisplays[i].TryChangeState(PlayerDisplay.State.Waiting);
            }
        }

        public void OnRound(Circuit.Round round)
        {
            _round = round;
            _levelSelectDisplay.SetActive(false);
            //_playerDisplay.SetActive(false);


            //round.OnRoundEndHandler += OnRoundEnd;
        }
        public void Update()
        {
            if(_round != null)
            _timer.Time = _round.Time;
        }

        public void OnRoundCountdown(int count)
        {
            _countDown.gameObject.SetActive(true);
            _countDown.Number = count;
        }

        public void OnIntermission(int roundNumber)
        {
            _timer.gameObject.SetActive(true);
            _roundDisplay.Number = roundNumber;
        }

        public void OnRoundEnd()
        {
            _timesUp.gameObject.SetActive(true);
            _timesUpTimer.Start();
        }


        private void OnTimesUpTimeOut()
        {
            _timesUp.gameObject.SetActive(false);
        }

        public void Join(Controls.Controller controller)
        {
            if (_availablePlayerDisplays.Count != 0)
            {
                _availablePlayerDisplays[0].TryChangeState(PlayerDisplay.State.Ready, controller.Number);
                _availablePlayerDisplays.RemoveAt(0);
                //_playerDisplays[index]?.TryChangeState(state);
            }

            
        }

        public void Leave(Controls.Controller controller)
        {
            if (controller.PlayerDisplay)
            {
                controller.PlayerDisplay.TryChangeState(PlayerDisplay.State.Waiting);
                _availablePlayerDisplays.Add(controller.PlayerDisplay);
                controller.PlayerDisplay = null;
            }
        }


        public void CountDown(int count)
        {
            _countDown.Number = count;
        }

        public void OnLevelSelect()
        {
            _countDown.gameObject.SetActive(false);
            _timer.gameObject.SetActive(false);
            _playerDisplay.SetActive(false);

            _levelSelectDisplay.SetActive(true);
        }

        public void OnScoreChanged(PlayerNumber player, float score)
        {
            _playerDisplays[(int)player].OnScoreChanged(score);
        }



        internal void OnLevelSelected(int step)
        {
            if (_game._currentLevelIndex == 0)
            {
                _previous.gameObject.SetActive(false);
            }
            else
            {
                _previous.gameObject.SetActive(true);
            }

            if (_game._currentLevelIndex == _game._levels.Length-1)
            {
                _next.gameObject.SetActive(false);
            }
            else
            {
                _next.gameObject.SetActive(true);
            }

            if (step < 0)
            {
                StartCoroutine(PunchScale(true));
            }
            else if(step > 0)
            {
                StartCoroutine(PunchScale(false));
            }

            if(_game._selectedLevel != null)
                _levelName.text = _game._selectedLevel.Name;

            foreach (var display in _playerDisplays)
            {
                display.TryChangeState(PlayerDisplay.State.Disabled);
            }            
        }

        public void OnValidate()
        {
            if (_game == null)
                _game = FindObjectOfType<Game>();

            OnLevelSelected(0);
        }
    }
}