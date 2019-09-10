using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Cirrus.Circuit.UI
{
    public class HUD : MonoBehaviour
    {
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

        private Round _round;


        public void Awake()
        {
            _availablePlayerDisplays = new List<PlayerDisplay>();
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

            for (int i = 0; i < Game.Instance.CurrentLevel.CharacterCount; i++)
            {
                _playerDisplays[i].TryChangeState(PlayerDisplay.State.Waiting);
            }
        }

        public void OnRound(Round round)
        {
            _round = round;
            _levelSelectDisplay.SetActive(false);
            //_playerDisplay.SetActive(false);

            _countDown.gameObject.SetActive(true);
            _timer.gameObject.SetActive(true);

            round.OnCountdownHandler += OnRoundCountdown;
            round.OnRoundBeginHandler += OnRoundBegin;
            round.OnRoundEndHandler += OnRoundEnd;
        }


        public void Update()
        {
            if(_round != null)
            _timer.Time = _round.Time;
        }

        public void OnRoundCountdown(int count)
        {
            _countDown.Number = count;
        }

        public void OnRoundBegin()
        {

        }

        public void OnRoundEnd()
        {

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


        internal void OnLevelSelected(int step)
        {
            if (Game.Instance._currentLevelIndex == 0)
            {
                _previous.gameObject.SetActive(false);
            }
            else
            {
                _previous.gameObject.SetActive(true);
            }

            if (Game.Instance._currentLevelIndex == Game.Instance._levels.Length-1)
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

            _levelName.text = Game.Instance.CurrentLevel.Name;

            foreach (var display in _playerDisplays)
            {
                display.TryChangeState(PlayerDisplay.State.Disabled);
            }            
        }

        public void OnValidate()
        {
            OnLevelSelected(0);
        }
    }
}