using UnityEngine;
using System.Collections;
using Cirrus.Circuit.Controls;
using Cirrus.Circuit.Networking;

namespace Cirrus.Circuit.UI
{
    public delegate void OnCharacterSelectReady(int numPlayers);

    public class CharacterSelect : BaseSingleton<CharacterSelect>
    {
        public OnCharacterSelectReady OnCharacterSelectReadyHandler;

        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("slots")]        
        private CharacterSelectSlot[] _slots;

        [SerializeField]        
        private UnityEngine.UI.Text _readyText;

        public int _readyCount = 0;

        public override void OnValidate()
        {
            base.OnValidate();

            if (_slots.Length == 0) _slots = GetComponentsInChildren<CharacterSelectSlot>();
        }

        public override void Awake()
        {
            base.Awake();

            Game.Instance.OnCharacterSelectHandler += OnCharacterSelect;
            //Game.Instance.OnLocalPlayerJoinHandler += OnPlayerJoin;
            Game.Instance.OnLevelSelectHandler += OnLevelSelect;
            Game.Instance.OnLevelSelectHandler += OnMenu;
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

        public void OnCharacterSelect(bool enabled)
        {
            Enabled = true;
        }

        public void OnLevelSelect(bool enabled)
        {
            Enabled = false;
        }

        public void OnMenu(bool enabled)
        {
            Enabled = false;
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
                    if (_state == State.Ready)
                    {
                        OnCharacterSelectReadyHandler?.Invoke(_readyCount);
                        return false;
                    }

                    if (_readyCount < 2) return false;       

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

        public void OnLocalCharacterScroll(int playerId, bool scroll)
        {
            _slots[playerId].CmdScroll(scroll);
        }


        public void AssignAuthority(Mirror.NetworkConnection conn, int playerId)
        {
            _slots[playerId].netIdentity.AssignClientAuthority(conn);                    
        }
    }
}
