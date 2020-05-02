using UnityEngine;
using System.Collections;
using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Mirror;
//using UnityEngine;
using Cirrus.MirrorExt;
using Cirrus.Utils;

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
        [SyncVar]
        private Timer _timer;

        [SyncVar]
        [SerializeField]
        private Timer _countDownTimer;

        [SyncVar]
        [SerializeField]
        private Timer _intermissionTimer;

        public float Time => _roundTime - _timer.Time;

        [SyncVar]
        [SerializeField]        
        private float _countDownTime = 1f;

        [SyncVar]
        [SerializeField]        
        private float _intermissionTime = 0; // Where we show the round number
        
        [SerializeField]
        private int _countDown = 3;

        [SyncVar]
        [SerializeField]
        private float _roundTime;

        [SyncVar]
        [SerializeField]
        private int _id = 0;

        public int Id => _id;

        private static RoundSession _instance;

        public override void OnStartServer()
        {
            base.OnStartServer();

            _countDownTimer.OnTimeLimitHandler += Cmd_OnTimeout;
            _intermissionTimer.OnTimeLimitHandler += Cmd_OnIntermissionTimeoutBeginCountdown;

        }

        public override void OnStartClient()
        {
            base.OnStartClient();            

            Game.Instance._SetState(Game.State.Round);

            BeginIntermission();
        }        

        public static RoundSession Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<RoundSession>();
                return _instance;
            }
        }

        public static RoundSession Create(
            int countDown, 
            float time, 
            float countDownTime, 
            float intermissionTime, 
            int id)
        {
            RoundSession session = NetworkingLibrary.Instance.RoundSession.Create(null);

            session._intermissionTime = intermissionTime;
            session._id = id;
            session._countDown = countDown;
            session._roundTime = time;
            session._countDownTime = countDownTime;
            session._countDownTimer = new Timer(
                countDownTime,
                start: false,
                repeat: true);

            session._timer = new Timer(
                session._roundTime,
                start: false);

            session._intermissionTimer = new Timer(
                session._intermissionTime,
                start: false,
                repeat: false);

            NetworkServer.Spawn(
                session.gameObject, 
                NetworkServer.localConnection);

            return session;
        }

        public void BeginIntermission()
        {
            OnIntermissionHandler?.Invoke(_id);
            if (CustomNetworkManager.IsServer)
            {
                _intermissionTimer.Start();
            }
        }


        [ClientRpc]
        public void Rpc_OnIntermissionTimeoutBeginCountdown()
        {
            _OnIntermissionTimeoutBeginCountdown();
        }

        public void _OnIntermissionTimeoutBeginCountdown()
        {
            OnCountdownHandler?.Invoke(_countDown);
            if(CustomNetworkManager.IsServer) _countDownTimer.Start();
        }


        public void Terminate()
        {
            _timer.Stop();
            _countDownTimer.Stop();
            _intermissionTimer.Stop();
            Cmd_OnRoundEnd();
        }


        private void Cmd_OnTimeout()
        {
            CommandClient.Instance.Cmd_RoundSession_OnTimeout(gameObject);
        }


        public void Cmd_OnIntermissionTimeoutBeginCountdown()
        {
            CommandClient.Instance.Cmd_OnIntermissionTimeoutBeginCountdown(gameObject);
        }

        [ClientRpc]
        public void Rpc_OnTimeout()
        {
            _OnTimeOut();
        }

        private void _OnTimeOut()
        {
            _countDown--;

            if (_countDown < -1)
            {
                OnCountdownHandler?.Invoke(_countDown);

                if(CustomNetworkManager.IsServer) _countDownTimer.Stop();
            }
            else if (_countDown < 0)
            {
                OnCountdownHandler?.Invoke(_countDown);
                OnRoundBeginHandler?.Invoke(_id);

                if (CustomNetworkManager.IsServer)
                {
                    _timer.Start();
                    _timer.OnTimeLimitHandler += Cmd_OnRoundEnd;
                }

                return;
            }
            else OnCountdownHandler?.Invoke(_countDown);
        }

        public void Cmd_OnRoundEnd()
        {
            CommandClient.Instance.Cmd_RoundSession_OnRoundEnd(gameObject);
        }

        [ClientRpc]
        public void Rpc_OnRoundEnd()
        {
            _OnRoundEnd();
        }

        public void _OnRoundEnd()
        {
            OnRoundEndHandler?.Invoke();
        }

    }
}
