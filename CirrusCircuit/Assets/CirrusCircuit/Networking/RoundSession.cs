using UnityEngine;
using System.Collections;
using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Mirror;
using UnityEngine;
using Cirrus.MirrorExt;

namespace Cirrus.Circuit.Networking
{
    public delegate void OnIntermission(int count);

    public delegate void OnCountdown(int count);

    public delegate void OnRoundBegin(int roundNumber);

    public delegate void OnRoundEnd();

    public class RoundSession : NetworkBehaviour
    {
        public OnIntermission OnIntermissionHandler;

        public OnCountdown OnCountdownHandler;

        public OnRoundBegin OnRoundBeginHandler;

        public OnRoundEnd OnRoundEndHandler;

        [SerializeField]        
        private Timer _timer;

        [SerializeField]
        private Timer _countDownTimer;
        
        [SerializeField]
        private Timer _intermissionTimer;

        public float Time => _roundTime - _timer.Time;

        [SyncVar]
        [SerializeField]        
        private float _countDownTime = 1f;

        [SyncVar]
        [SerializeField]        
        private float _intermissionTime = 0; // Where we show the round number

        [SyncVar]
        [SerializeField]        
        private int _countDown;

        [SyncVar]
        [SerializeField]
        private float _roundTime;

        [SyncVar]
        [SerializeField]
        private int _id = 0;

        public int Id => _id;

        private static RoundSession _instance;

        public static RoundSession Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<RoundSession>();
                return _instance;
            }
        }

        public static RoundSession Create(int countDown, float time, float countDownTime, float intermissionTime, int id)
        {
            RoundSession session = null;

            if (ServerUtils.TryCreateNetworkObject(
                NetworkServer.localConnection,
                NetworkingLibrary.Instance.RoundSession.gameObject,
                out NetworkIdentity obj, 
                false))
             {
                session = obj.GetComponent<RoundSession>();
                if (session != null)
                {

                    session._intermissionTime = intermissionTime;
                    session._id = id;
                    session._countDown = countDown;
                    session._roundTime = time;
                    session._countDownTime = countDownTime;
                    session._countDownTimer = new Timer(countDownTime, start: false, repeat: true);
                    session._countDownTimer.OnTimeLimitHandler += session.OnTimeOut;
                    session._timer = new Timer(session._roundTime, start: false);

                    session._intermissionTimer = new Timer(session._intermissionTime, start: false, repeat: false);
                    session._intermissionTimer.OnTimeLimitHandler += session.OnIntermissionTimeoutBeginCountdown;
                }
            }

            return session;
        }

        public void BeginIntermission()
        {
            OnIntermissionHandler?.Invoke(_id);
            _intermissionTimer.Start();
        }
        
        public void OnIntermissionTimeoutBeginCountdown()
        {
            OnCountdownHandler?.Invoke(_countDown);
            _countDownTimer.Start();
        }

        public void Terminate()
        {
            _timer.Stop();
            _countDownTimer.Stop();
            _intermissionTimer.Stop();
            OnRoundEnd();
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
                OnRoundBeginHandler.Invoke(_id);
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
