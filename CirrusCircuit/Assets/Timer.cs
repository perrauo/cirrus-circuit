using UnityEngine;
using System.Collections;

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

        bool active = false;
        public bool IsActive
        {
            get
            {
                return active;
            }
        }

        public OnTimeLimit OnTimeLimitHandler;

        public Timer(float limit, bool start = true, bool repeat = false)
        {
            _limit = limit;
            _repeat = repeat;

            if (start)
            {
                Start();
            }
        }

        public void Reset()
        {
            _time = 0;
        }

        public void Start()
        {
            Reset();

            active = true;
            Game.Instance.Clock.OnTickedHandler += OnTicked;
        }

        public void Resume()
        {
            active = true;
            Game.Instance.Clock.OnTickedHandler += OnTicked;
        }


        public void Stop()
        {
            active = false;
            Game.Instance.Clock.OnTickedHandler -= OnTicked;
        }

        private void OnTicked()
        {
            _time += Time.deltaTime;
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
