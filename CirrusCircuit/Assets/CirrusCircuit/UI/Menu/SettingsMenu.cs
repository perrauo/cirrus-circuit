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
        private Toggle _areGemsSpawnedToggle;

        [SerializeField]
        private SliderField _numberOfRoundSlider;

        [SerializeField]
        private SliderField _roundTimeSlider;

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

            _roundTimeSlider.OnValueChangedHandler += (x) => Settings.RoundTime.Set((int)x);

            _numberOfRoundSlider.OnValueChangedHandler += (x) => Settings.NumberOfRounds.Set((int)x);

            _areGemsSpawnedToggle.onValueChanged.AddListener(x => Settings.AreGemsSpawned.Set(x));

            _areControlsBoundToDirectionToggle.onValueChanged.AddListener(x => Settings.IsUsingAlternateControlScheme.Set(x));

            Enabled = false;
        }
        
        public override void Start()
        {
            base.Start();

            _roundTimeSlider.Value = Settings.RoundTime.Integer;
            _numberOfRoundSlider.Value = Settings.NumberOfRounds.Integer;
            _areControlsBoundToDirectionToggle.SetIsOnWithoutNotify(Settings.IsUsingAlternateControlScheme.Boolean);
            _areGemsSpawnedToggle.SetIsOnWithoutNotify(Settings.AreGemsSpawned.Boolean);            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}