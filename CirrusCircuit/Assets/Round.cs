using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit
{
    public delegate void OnIntermission(int count);

    public delegate void OnCountdown(int count);

    public delegate void OnRoundBegin(int roundNumber);

    public delegate void OnRoundEnd();

    public class Round
    {
        public OnIntermission OnIntermissionHandler;

        public OnCountdown OnCountdownHandler;

        public OnRoundBegin OnRoundBeginHandler;

        public OnRoundEnd OnRoundEndHandler;

        private Timer _timer;

        private Timer _countDownTimer;

        private float _intermissionTime = 0; // Where we show the round number

        private Timer _intermissionTimer;

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


        private int _number = 0;


        public int Number
        {
            get {
                return _number;
            }
        }

        public Round(int countDown, float time, float countDownTime, float intermissionTime, int number)
        {
            _intermissionTime = intermissionTime;

            _number = number;

            _countDown = countDown;

            _roundTime = time;
            _countDownTime = countDownTime;

            _countDownTimer = new Timer(countDownTime, start:false, repeat:true);
            _countDownTimer.OnTimeLimitHandler += OnTimeOut;

            _timer = new Timer(_roundTime, start:false);

            _intermissionTimer = new Timer(_intermissionTime, start: false, repeat:false);
            _intermissionTimer.OnTimeLimitHandler += OnIntermissionTimeoutBeginCountdown;
        }

        public void BeginIntermission()
        {
            OnIntermissionHandler?.Invoke(_number);
            _intermissionTimer.Start();
        }
        
        public void OnIntermissionTimeoutBeginCountdown()
        {
            OnCountdownHandler?.Invoke(_countDown);
            _countDownTimer.Start();
        }

        private void OnTimeOut()
        {
            _countDown--;

            if (_countDown < -1)
            {
                OnCountdownHandler?.Invoke(_countDown);
                _countDownTimer.Stop();
            }
            else if (_countDown < 0)
            {
                OnCountdownHandler?.Invoke(_countDown);
                OnRoundBeginHandler.Invoke(_number);
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
