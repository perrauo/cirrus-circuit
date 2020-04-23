using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit.UI
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Button _playButton;

        [SerializeField]
        private UnityEngine.UI.Button _exitButton;

        [SerializeField]
        private UnityEngine.UI.Button _hostButton;

        [SerializeField]
        private UnityEngine.UI.Button _joinButton;

        [SerializeField]
        private UnityEngine.UI.InputField _joinInput;


        public void OnValidate()
        {

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


        public void Awake()
        {
            _exitButton.onClick.AddListener(OnExitClick);
            _playButton.onClick.AddListener(Game.Instance.OnStartClicked);
            Game.Instance.OnCharacterSelectHandler += OnCharacterSelect;
        }

        public void OnExitClick()
        {

        }


        public void OnCharacterSelect(bool enabled)
        {
            Enabled = false;
        }

    }
}
