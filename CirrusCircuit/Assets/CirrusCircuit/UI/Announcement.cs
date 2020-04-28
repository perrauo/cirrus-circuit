using UnityEngine;
using System.Collections;
using Cirrus.Circuit.Networking;

namespace Cirrus.Circuit.UI
{
    public class Announcement : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Text _text;

        private int _number = 0;

        public void OnValidate()
        {
            //if (Game.Instance == null)
            //    Game.Instance = FindObjectOfType<Game>();
        }

        public int RoundNumber {
            get {
                return _number;
            }

            set
            {
                Enabled = true;
                _number = value;
                _text.text = "Round " + (_number + 1).ToString();
                _timer.Start();
            }

        }

        private string _message = "";

        public string Message
        {
            get
            {
                return _message;
            }

            set
            {
                Enabled = true;
                _message = value;
                _text.text = _message;
                _timer.Start();
            }

        }



        private bool _enabled = false;

        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                _enabled = value;
                transform.GetChild(0).gameObject.SetActive(_enabled);
            }
        }

        private float _time = 2f;

        private Circuit.Timer _timer;


        private Circuit.Timer _timesUpTimer;

        [SerializeField]
        private float _timesUpTime = 2f;

        public void Awake()
        {
            _timesUpTimer = new Circuit.Timer(_timesUpTime, start: false, repeat: false);
            _timesUpTimer.OnTimeLimitHandler += OnTimesUpTimeOut;

            _timer = new Circuit.Timer(_time, start: false, repeat: false);
            _timer.OnTimeLimitHandler += OnTimeOut;

            GameSession.OnStartClientStaticHandler += OnClientStarted;
            GameSession.Instance.OnNewRoundHandler += OnNewRound;
        }

        public void OnClientStarted(bool enable)
        {
            if (enable)
            {
                GameSession.Instance.OnNewRoundHandler += OnNewRound;
            }
            else
            {
                GameSession.Instance.OnNewRoundHandler -= OnNewRound;
            }
        }

        public void OnNewRound(RoundSession round)
        {
            round.OnRoundEndHandler += OnRoundEnd;
            round.OnIntermissionHandler += OnIntermission;
        }

        public void OnIntermission(int count)
        {
            RoundNumber = count;
        }

        public void OnRoundEnd()
        {
            Enabled = true;
            _timesUpTimer.Start();
        }

        public void OnTimeOut()
        {
            Enabled = false;
        }

        private void OnTimesUpTimeOut()
        {
            Enabled = false;
        }
    }
}