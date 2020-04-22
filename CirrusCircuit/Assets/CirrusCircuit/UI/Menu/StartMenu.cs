using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit.UI
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Button _buttonPlay;

        [SerializeField]
        private UnityEngine.UI.Button _buttonExit;




        public void OnValidate()
        {

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


        public void Awake()
        {
            _buttonExit.onClick.AddListener(OnExitClick);
            _buttonPlay.onClick.AddListener(Game.Instance.OnStartClicked);
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
