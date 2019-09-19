using UnityEngine;
using System.Collections;
using Cirrus.Circuit.Controls;

namespace Cirrus.Circuit.UI
{
    public class CharacterSelect : MonoBehaviour
    {
        [SerializeField]
        private Game _game;

        [SerializeField]
        private CharacterSelectSlot[] slots;

        [SerializeField]
        private UnityEngine.UI.Text _readyText;

        public int _readyCount = 0;

        public void OnValidate()
        {
            if (_game == null)
                _game = FindObjectOfType<Game>();

            if (slots.Length == 0)
                slots = GetComponentsInChildren<CharacterSelectSlot>();
        }

        public void Awake()
        {
            _game.OnCharacterSelectHandler += OnCharacterSelect;
            _game.OnControllerJoinHandler += OnControllerJoin;
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

        public void OnCharacterSelect(bool enabled)
        {
            Enabled = true;
        }

        public enum State
        {
            Disabled,
            Select,
            Ready,
        }

        private State _state;

        public bool TryChangeState(State target)
        {
            switch (target)
            {
                case State.Disabled:
                    _state = target;
                    return true;

                case State.Ready:
                    if (_readyCount < 2)
                    {
                        return false;
                    }

                    _readyText.text = "Ready?";
                    _state = target;
                    return true;


                case State.Select:
                    _readyText.text = "";
                    _state = target;
                    return true;
            }

            return false;
        }


        public void HandleAction1()
        {

        }

        public void HandleAction0()
        {

        }


        public void OnControllerJoin(Controller controller)
        {
            controller._characterSlot = slots[controller.Number];
            slots[controller.Number].TryChangeState(CharacterSelectSlot.State.Selecting);
        }
    }
}
