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

    public class RoundSession : CustomNetworkBehaviour
    {
        public OnIntermission OnIntermissionHandler;

        public OnCountdown OnCountdownHandler;

        public OnRoundBegin OnRoundStartHandler;

        public OnRoundEnd OnRoundEndHandler;

        [SyncVar]
        [SerializeField]
        private GameObject _timerGameObject;
        private ServerTimer _timer;
        public ServerTimer Timer
        {
            get {
                if (_timer == null) _timer = _timerGameObject.GetComponent<ServerTimer>();
                return _timer;
            }
        }



        [SyncVar]
        [SerializeField]
        private Timer _countDownTimer;

        [SyncVar]
        [SerializeField]
        private Timer _intermissionTimer;

        public float RemainingTime => _remainingTime - Timer.Time;

        [SyncVar]
        [SerializeField]        
        private float _countDownTime = 1f;

        [SyncVar]
        [SerializeField]        
        private float _intermissionTime = 0; // Where we show the round number
        
        [SyncVar]
        [SerializeField]
        private int _countDown = 3;

        [SyncVar]
        [SerializeField]
        private float _remainingTime;

        [SyncVar]
        [SerializeField]
        private int _index = 0;

        public int Index => _index;

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
            int index)
        {
            RoundSession session = NetworkingLibrary.Instance.RoundSession.Create(null);

            session._intermissionTime = intermissionTime;
            session._index = index;
            session._countDown = countDown;
            session._remainingTime = time;
            session._countDownTime = countDownTime;
            session._countDownTimer = new Timer(
                countDownTime,
                start: false,
                repeat: true);

            if (CustomNetworkManager.IsServer)
            {
                session._timerGameObject = ServerTimer.Create(
                    session._remainingTime,
                    start: false).gameObject;
            }

            session._intermissionTimer = new Timer(
                session._intermissionTime,
                start: false,
                repeat: false);

            NetworkServer.Spawn(
                session.gameObject, 
                NetworkServer.localConnection);

            return session;
        }

        public void StartIntermisison()
        {
            OnIntermissionHandler?.Invoke(_index);
            if (CustomNetworkManager.IsServer) _intermissionTimer.Start();
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
            if(CustomNetworkManager.IsServer) Timer.Stop();

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
                OnRoundStartHandler?.Invoke(_index);

                if (CustomNetworkManager.IsServer)
                {
                    Timer.DoStart();
                    Timer.OnTimeLimitHandler += Cmd_OnRoundEnd;
                }

                return;
            }
            else OnCountdownHandler?.Invoke(_countDown);
        }

        public void Cmd_OnRoundEnd()
        {
            if (CustomNetworkManager.IsServer) 
            {                
                NetworkServer.Destroy(_timerGameObject);
                Destroy(_timerGameObject);
                _timerGameObject = null;
            }

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
