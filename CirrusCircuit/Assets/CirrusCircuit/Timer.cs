using UnityEngine;
using System.Collections;
using System;

// TODO: use in cooldown


namespace Cirrus.Circuit
{
    public delegate void OnTimeLimit();

    // A timer cannot be created in Start(), or Wake() because it needs the Clock, instead to create duiring init use OnEnable    
    public class Timer
    {
        bool _repeat = false;
        float _limit = -1;

        float _time = 0f;
        public float Time => _time;

        bool _isactive = false;
        public bool IsActive => _isactive;        

        public OnTimeLimit OnTimeLimitHandler;

        public Timer(float limit, bool start = true, bool repeat = false)
        {
            _time = 0;
            _limit = limit;
            _repeat = repeat;

            if (start)
            {
                Start();
            }
        }        

        public void Reset(float limit=-1)
        {
            if (limit > 0)
            {
                _limit = limit;
            }

            _time = 0;
        }

        public void Start()
        {
            Reset();

            if (!_isactive)
            {
                Clock.Instance.OnTickedHandler += OnTicked;
            }

            _isactive = true;
        }

        public void Resume()
        {
            if (!_isactive)
            {
                Clock.Instance.OnTickedHandler += OnTicked;
            }

            _isactive = true;
        }


        public void Stop()
        {
            if (_isactive)
            {
                Clock.Instance.OnTickedHandler -= OnTicked;
            }

            _isactive = false;
        }

        private void OnTicked()
        {
            _time += UnityEngine.Time.deltaTime;
            if (_time >= _limit)
            {
                OnTimeLimitHandler?.Invoke();

                if (_repeat)
                {
                    Reset();
                }
                else
                {
                    Stop();
                }         
            }
        }

        ~Timer()
        {
            Stop();
        }
    }
}
