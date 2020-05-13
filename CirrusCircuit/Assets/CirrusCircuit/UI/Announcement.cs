using UnityEngine;
using System.Collections;
using Cirrus.Circuit.Networking;

namespace Cirrus.Circuit.UI
{
    public class Announcement : BaseSingleton<Announcement>
    {
        [SerializeField]
        private UnityEngine.UI.Text _text;

        private int _index = 0;

        public override void OnValidate()
        {
            base.OnValidate();

            //if (Game.Instance == null)
            //    Game.Instance = FindObjectOfType<Game>();
        }

        public int RoundIndex {
            get => _index;

            set
            {
                Enabled = true;
                _index = value;
                _text.text = "Round " + (_index + 1).ToString();
                _timer.Start();
            }
        }

        private string _message = "";

        public string Message
        {
            get => _message;           

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
            get => _enabled;            

            set
            {
                _enabled = value;
                transform.GetChild(0).gameObject.SetActive(_enabled);
            }
        }

        [SerializeField]
        private float _time = 1f;

        private Circuit.Timer _timer;


        public override void Awake()
        {
            base.Awake();

            _timer = new Circuit.Timer(
                _time, 
                start: false, 
                repeat: false);
            _timer.OnTimeLimitHandler += OnTimeOut;
        }

        public void OnTimeOut()
        {
            Enabled = false;
        }

    }
}