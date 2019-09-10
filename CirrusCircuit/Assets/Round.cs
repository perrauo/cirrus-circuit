using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit
{
    public delegate void OnCountdown(int count);

    public delegate void OnRoundBegin();

    public delegate void OnRoundEnd();

    public class Round
    {
        public OnCountdown OnCountdownHandler;

        public OnRoundBegin OnRoundBeginHandler;

        public OnRoundEnd OnRoundEndHandler;

        private Timer _timer;

        private Timer _countDownTimer;

        public float Time
        {
            get
            {
                return _roundTime -_timer.Time;
            }
        }

        private float _countDownTime = 1f;

        private int _countDown;

        private float _roundTime;

        public Round(int countDown, float time, float countDownTime)
        {
            _countDown = countDown;
            _roundTime = time;
            _countDownTime = countDownTime;

            _countDownTimer = new Timer(countDownTime, repeat:true);
            _countDownTimer.OnTimeLimitHandler += OnTimeOut;

            _timer = new Timer(_roundTime, false);
        }

        private void OnTimeOut()
        {
            _countDown--;

            if (_countDown < -1)
            {
                OnRoundBeginHandler.Invoke();
                _countDownTimer.Stop();
                _timer.Start();
                _timer.OnTimeLimitHandler += OnRoundEnd;
                return;
            }
            else
            {
                OnCountdownHandler?.Invoke(_countDown);
            }
        }

        public void OnRoundEnd()
        {
            OnRoundEndHandler.Invoke();
        }

    }
}
