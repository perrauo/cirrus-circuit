using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit.World.Objects
{
    public class Door : BaseObject
    {
        public delegate void OnScoreValueAdded(
        Controls.PlayerNumber playerNumber,
        float score
        );

        public OnScoreValueAdded OnScoreValueAddedHandler;

        [SerializeField]
        private PlayfulSystems.ProgressBar.ProgressBarPro _progressBar;

        [SerializeField]
        private int _comboRequired = 2;

        [SerializeField]
        private int _comboAmount = 0;

        private Gem _previousGem;

        private float _multiplier = 1;

        [SerializeField]
        private float _multiplierIncrement = 0.5f;

        private Timer _multiplierTimer;

        [SerializeField]
        private float _multiplierTime = 2f;

        private Timer _valueTimer;

        [SerializeField]
        private float _valueTime = 1.5f;

        [SerializeField]
        private float _scorePunchScaleAmount = 0.5f;

        [SerializeField]
        private float _scorePunchScaleTime = 1f;

        [SerializeField]
        private UnityEngine.UI.Text _textMultiplier;

        [SerializeField]
        private UnityEngine.UI.Text _textValue;

        [SerializeField]
        private float _punchScaleAmount = 1f;

        [SerializeField]
        private float _punchScaleTime = 1f;

  
        IEnumerator PunchScale()
        {
            iTween.Stop(_visual.Parent.gameObject);
            _visual.Parent.gameObject.transform.localScale = new Vector3(1, 1, 1);

            yield return new WaitForSeconds(0.01f);

            iTween.PunchScale(_visual.Parent.gameObject,
                new Vector3(_punchScaleAmount, _punchScaleAmount, _punchScaleAmount),
                _punchScaleTime);

            yield return null;
        }

        protected override void Awake()
        {
            base.Awake();

            _multiplierTimer = new Timer(_multiplierTime, start: false, repeat: false);
            _multiplierTimer.OnTimeLimitHandler += OnMultiplierTimeOut;


            _valueTimer = new Timer(_valueTime, start: false, repeat: false);
            _valueTimer.OnTimeLimitHandler += OnValueTimeOut;

            Multiplier = 1;

            Value = 0;           

        }

        public override void Update()
        {
            base.Update();

            _progressBar.SetValue(1 - (_multiplierTimer.Time / _multiplierTime));
        }


        public override bool TryMove(Vector3Int step, BaseObject incoming = null)
        {
            return false;
        }


        public override bool TryEnter(Vector3Int step, ref Vector3 offset, BaseObject incoming = null)
        {
            if (base.TryEnter(step, ref offset, incoming))
            {
                //_user = incoming;

                switch (incoming.Id)
                {
                    case ObjectId.Gem:

                        iTween.Init(_visual.Parent.gameObject);
                        iTween.Stop(_visual.Parent.gameObject);

                        _visual.Parent.gameObject.transform.localScale = new Vector3(1, 1, 1);
                        StartCoroutine(PunchScale());

                        OnGemEntered(incoming as Gem);

                        incoming._targetScale = 0;
                        offset += Vector3.up * World.Level.GridSize / 2;

                        return true;

                    case ObjectId.Character:
                        return false;
                    default:
                        return false;
                }
            }

            return false;
        }


        public override bool TryFall(BaseObject incoming = null)
        {
            return false;
        }


        public override void Accept(BaseObject incoming)
        {
            switch (incoming.Id)
            {
                case ObjectId.Gem:
                    iTween.Init(Object);
                    iTween.Stop(Object);
                    
                    //_visual.Parent.transform.localScale = new Vector3(1, 1, 1);
                    //StartCoroutine(PunchScale());
                    //incoming._targetScale = 0;

                    return;
                default:
                    return;
            }
        }


        IEnumerator PunchValue()
        {
            iTween.Stop(_textValue.gameObject);
            _textValue.gameObject.transform.localScale = new Vector3(1, 1, 1);

            yield return new WaitForSeconds(0.01f);

            iTween.PunchScale(_textValue.gameObject,
                new Vector3(_scorePunchScaleAmount, _scorePunchScaleAmount, _scorePunchScaleAmount),
                _scorePunchScaleTime);


            yield return null;
        }

        IEnumerator PunchMultiplier()
        {
            iTween.Stop(_textMultiplier.gameObject);
            _textMultiplier.gameObject.transform.localScale = new Vector3(1, 1, 1);

            yield return new WaitForSeconds(0.01f);

            iTween.PunchScale(_textMultiplier.gameObject,
                new Vector3(_scorePunchScaleAmount, _scorePunchScaleAmount, _scorePunchScaleAmount),
                _scorePunchScaleTime);


            yield return null;
        }



        public void OnGemEntered(Gem gem)
        {
            if (_previousGem != null &&
                gem.PlayerNumber == _previousGem.PlayerNumber &&
                gem.Type == _previousGem.Type)
            {
                _multiplierTimer.Start();
                _comboAmount++;
            }
            else
            {
                _comboAmount = 0;
                Multiplier = 1;
                _multiplierTimer.Stop();
            }

            iTween.Init(_textValue.gameObject); //TODO move elsewhere
            iTween.Stop(_textValue.gameObject);
            _textValue.gameObject.transform.localScale = new Vector3(1, 1, 1);

            Value = gem.Value * _multiplier;

            _valueTimer.Start();

            StartCoroutine(PunchValue());

            OnScoreValueAddedHandler?.Invoke(PlayerNumber, Value);

            _previousGem = gem;

            if (_comboAmount % _comboRequired == _comboRequired -1)
            {
                Multiplier += _multiplierIncrement;

                iTween.Init(_textMultiplier.gameObject); //TODO move elsewhere
                iTween.Stop(_textMultiplier.gameObject);
                _textMultiplier.gameObject.transform.localScale = new Vector3(1, 1, 1);

                StartCoroutine(PunchMultiplier());
            }
        }



        public void OnMultiplierTimeOut()
        {
            _comboAmount = 0;
            _previousGem = null;
            Multiplier = 1;
        }

        private void OnValueTimeOut()
        {
            Value = 0;
        }


        public float Multiplier
        {
            get
            {
                return _multiplier;
            }

            set
            {
                _multiplier = value;

                if (_multiplier <= 1)
                {
                    _textMultiplier.text = "";
                    _progressBar.gameObject.SetActive(false);
                }
                else
                {
                    _textMultiplier.text = " x" + _multiplier.ToString() + "";
                    _progressBar.gameObject.SetActive(true);
                }
            }

        }

        private float _value;

        public float Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;

                if (_value <= 0)
                {
                    _textValue.text = "";
                }
                else
                {
                    _textValue.text = " +" + _value.ToString();
                }
                
            }

        }


    }
}