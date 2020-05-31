using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cirrus.UI;
using UnityEngine.UI;

namespace Cirrus.Circuit.UI
{

    public class SettingsMenu : BaseSingleton<SettingsMenu>
    {
        [SerializeField]
        private Toggle _areControlsBoundToDirectionToggle;

        [SerializeField]
        private Toggle _isGemSpawnEnabledToggle;

        [SerializeField]
        private IntegerSliderField _numberOfRoundSlider;

        [SerializeField]
        private IntegerSliderField _roundTimeSlider;

        [SerializeField]
        private Button _backButton;

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

        public override void Awake()
        {
            base.Awake();

            _backButton.onClick.AddListener(() => {
                Enabled = false;
                StartMenu.Instance.Enabled = true;

            });

            Enabled = false;
        }

        public override void Start()
        {
            base.Start();                
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}