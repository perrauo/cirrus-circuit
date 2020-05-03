using UnityEngine;
using System.Collections;
using System;
using Mirror;
using Cirrus.Circuit.Networking;
using Cirrus.Utils;

namespace Cirrus.Circuit
{
    [Serializable]
    public class ServerTimer : NetworkBehaviour
    {
        [SerializeField]
        bool _repeat = false;

        [SyncVar]
        [SerializeField]
        float _limit = -1;

        public const float DefaultLimit = 0.5f;

        [SyncVar]
        [SerializeField]
        float _time = 0f;

        [SerializeField]
        private bool _isFixedUpdate;

        bool active = false;
        public bool IsActive => active;

        [NonSerialized]
        public Events.Event<float> OnTickHandler;

        [NonSerialized]
        public Events.Event OnTimeLimitHandler;

        private Events.Event OnClockUpdateHandler
        {
            get
            {
                return _isFixedUpdate ?
                    CommandClient.Instance.OnFixedUpdateHandler :
                    CommandClient.Instance.OnUpdateHandler;
            }

            set
            {
                if (_isFixedUpdate) CommandClient.Instance.OnFixedUpdateHandler = value;
                else CommandClient.Instance.OnUpdateHandler = value;
            }
        }

        public static ServerTimer Create(float limit, bool start = true, bool repeat = false, bool fixedUpdate = false)
        {
            var timer = NetworkingLibrary.Instance.ServerTimer.Create(null);
            timer._time = 0;
            timer._limit = limit;
            timer._repeat = repeat;
            timer._isFixedUpdate = fixedUpdate;

            NetworkServer.Spawn(timer.gameObject);
            if (start) timer.DoStart();
            return timer;
        }

        public float Time => _time;

        public void Reset(float limit = -1)
        {
            if (limit > 0) _limit = limit;

            _time = 0;
        }

        public void DoStart(float limit = -1)
        {
            Reset(limit);

            if (!active) OnClockUpdateHandler += OnTicked;

            active = true;
        }

        public void Resume()
        {
            if (!active) OnClockUpdateHandler += OnTicked;            

            active = true;
        }

        public void Stop()
        {
            if (active) OnClockUpdateHandler -= OnTicked;

            active = false;
        }

        private void OnTicked()
        {
            _time += UnityEngine.Time.deltaTime;
            OnTickHandler?.Invoke(_time);
            if (_time >= _limit)
            {
                _time = _limit;

                OnTimeLimitHandler?.Invoke();

                if (_repeat) Reset();
                else Stop();
            }
        }

        ~ServerTimer()
        {
            Stop();
        }

    }
}
