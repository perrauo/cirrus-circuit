using UnityEngine;
using System.Collections;
using Cirrus.Circuit.Controls;
using Cirrus.Circuit.Networking;
using UnityEditor;

namespace Cirrus.Circuit.UI
{
    public delegate void OnCharacterSelectReady(int numPlayers);

    public class CharacterSelectInterface : BaseSingleton<CharacterSelectInterface>
    {
        public enum State
        {
            Disabled,
            Select,
            Ready,
        }

        private State _state;

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

        #region Unity Engine

        public override void OnValidate()
        {
#if UNITY_EDITOR

            base.OnValidate();

            if (_slots.Length == 0)
            {
                _slots = GetComponentsInChildren<CharacterSelectSlot>();

                for (int i = 0; i < _slots.Length; i++)
                {
                    _slots[i]._index = i;
                    EditorUtility.SetDirty(_slots[i]);
                }
            }
#endif

        }

        public override void Awake()
        {
            base.Awake();

            Game.Instance.OnCharacterSelectHandler += OnCharacterSelect;

            OnCharacterSelectReadyHandler += Game.Instance.OnCharacterSelected;
        }

        #endregion


        #region Mirror        


        public void OnClientStarted(bool enable)
        {

        }       

        public void SetAuthority(Mirror.NetworkConnection conn, int serverPlayerId)
        {
            CharacterSelectSlot playerSlot = _slots[serverPlayerId];
            if (playerSlot != null)
            {
                playerSlot.SetAuthority(conn, serverPlayerId);
                playerSlot.Cmd_SetState(CharacterSelectSlotState.Selecting);
            }
        }

        #endregion

        public void OnCharacterSelect(bool enable)
        {
            Enabled = enable;
        }

        public void HandleAction1()
        {

        }

        public void HandleAction0()
        {

        }


        public bool SetState(State target)
        {
            switch (target)
            {
                case State.Disabled:
                    _state = target;
                    return true;

                case State.Ready:
                    if (_state == State.Ready)
                    {
                        OnCharacterSelectReadyHandler
                            ?.Invoke(GameSession
                            .Instance
                            .CharacterSelectReadyCount);

                        return false;
                    }

                    if (GameSession
                        .Instance
                        .CharacterSelectReadyCount == 1 ||
                        GameSession
                        .Instance
                        .CharacterSelectReadyCount != GameSession.Instance.CharacterSelectOpenCount)
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
    }
}
