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
        public CharacterSelectSlot[] _slots;

        [SerializeField]        
        private UnityEngine.UI.Text _readyText;


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


        public override void OnValidate()
        {
            base.OnValidate();

            if (_slots.Length == 0) _slots = GetComponentsInChildren<CharacterSelectSlot>();
        }

        public override void Awake()
        {
            base.Awake();

            GameSession.OnStartClientStaticHandler += OnClientStarted;
            Game.Instance.OnCharacterSelectHandler += OnCharacterSelect;
        }

        public void OnCharacterSelect(bool enable)
        {
            Enabled = enable;
        }

        public void OnClientStarted(bool enable)
        {

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
                        OnCharacterSelectReadyHandler?.Invoke(GameSession.Instance.CharacterSelectReadyCount);
                        return false;
                    }

                    if (GameSession.Instance.CharacterSelectReadyCount == 1 ||
                        GameSession.Instance.CharacterSelectReadyCount != GameSession.Instance.CharacterSelectOpenCount)
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

        //public void OnLocalCharacterScroll(int playerId, bool scroll)
        //{
        //    _slots[playerId].CmdScroll(scroll);
        //}


        public void AssignAuthority(Mirror.NetworkConnection conn, int serverPlayerId)
        {
            _slots[serverPlayerId].netIdentity.AssignClientAuthority(conn);                    
        }
    }
}
