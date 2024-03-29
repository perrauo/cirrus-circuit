﻿using UnityEngine;
using System.Collections;
using System;
using Cirrus.Circuit.Networking;

namespace Cirrus.Circuit.UI
{

    public class Timer : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Text _text;

        private RoundSession _round;

        public float TimeRemaining
        {
            set
            {
                var span = new TimeSpan(0, 0, (int)value);
                _text.text = string.Format(span.ToString(@"mm\:ss"));
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

        public void OnValidate()
        {

        }

        public void Awake()
        {
            GameSession.OnStartClientStaticHandler += OnClientStarted;
            Game.Instance.OnRoundInitHandler += OnRoundInit;
            Game.Instance.OnRoundHandler += OnRound;
            //Game.Instance.OnR
        }

        public void OnClientStarted(bool enable)
        {

        }

        public void Update()
        {
            if (RoundSession.Instance != null) TimeRemaining = RoundSession.Instance.RemainingTime;
        }

        public void OnRoundInit()
        {            
            //RoundSession.Instance.OnIntermissionHandler += OnIntermission;
        }

        public void OnRound()
        {
            Enabled = true;
        }

        public void OnRoundEnded()
        {            
            Enabled = false;
        }
    }
}
