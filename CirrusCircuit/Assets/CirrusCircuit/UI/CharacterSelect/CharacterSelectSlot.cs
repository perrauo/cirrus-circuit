using UnityEngine;
using System.Collections;

using Cirrus.Circuit.World.Objects.Characters;
using Cirrus;
using System.Collections.Generic;
using UnityEngine.UI;
using Mirror;

using Cirrus.Circuit.Networking;
using System;

namespace Cirrus.Circuit.UI
{
    [Serializable]
    public enum CharacterSelectSlotState
    {
        Unknown,
        Closed,
        Ready,
        Selecting,
    }

    public class CharacterSelectSlot : NetworkBehaviour
    {
        [SerializeField]
        private Color _readyColor = Color.white;

        [SerializeField]
        private Color _joinColor = Color.black;

        [SerializeField]        
        private RawImage _imageTemplate;

        [SerializeField]
        private List<RawImage> _portraits;

        [SerializeField]        
        private RectTransform _rect;

        [SerializeField]
        private RectTransform _maskRect;

        [SerializeField]
        [SyncVar]
        private float _offset = -512;

        [SerializeField]
        [SyncVar]
        private float _portraitHeight = 256;

        [SyncVar]
        [SerializeField] float _totalHeight = 0;

        [SerializeField]
        [SyncVar]
        private float _bound = 0;

        [SerializeField]        
        private GameObject _selection;

        [SerializeField]        
        private Text _statusText;

        [SerializeField]        
        private Text _up;

        [SerializeField]        
        private Text _down;

        [SerializeField]
        [SyncVar]
        private float _speed = 0.5f;

        [SerializeField]
        [SyncVar]
        private float _selectPunchScale = 0.5f;

        [SerializeField]
        [SyncVar]
        private float _selectPunchScaleTime = 1f;

        [SerializeField]
        [SyncVar]
        private Vector3 _startPosition;

        [SerializeField]
        [SyncVar]
        private Vector3 _targetPosition;

        [SerializeField]
        private Transform _characterSpotlightAnchor;

        [SerializeField]
        [SyncVar]
        private float _characterSpotlightSize = 10f;

        [SerializeField]
        [SyncVar]
        private float _characterSpotlightRotateSpeed = 20f;

        [SerializeField]
        [SyncVar]
        private int _selectedIndex = 0;

        [SerializeField]
        [SyncVar]
        private float _disabledArrowAlpha = 0.35f;

        [SerializeField]
        [SyncVar]
        private CharacterSelectSlotState _state = CharacterSelectSlotState.Unknown;
        public CharacterSelectSlotState State => _state;

        [SerializeField]
        private CameraController _camera;

        [SyncVar]
        [SerializeField]
        public int _serverPlayerId = -1;
        public int ServerPlayerId {
            get => _serverPlayerId;
            set {

                _serverPlayerId = value;
                CommandClient.Instance.Cmd_CharacterSelectSlot_SetPlayerServerId(gameObject, _serverPlayerId);
            }
        }

        #region Unity Engine

        private void OnValidate()
        {
            if (_camera == null) _camera = FindObjectOfType<CameraController>();
            if (_rect == null) _rect = _selection.GetComponent<RectTransform>();            
        }

        public virtual void Awake()
        {
            _serverPlayerId = -1;
        }

        public virtual void Start()
        {            
            if (_imageTemplate == null) DebugUtils.Assert(false, "Portrait template is null");
            else _imageTemplate.gameObject.SetActive(true);
            _portraits = new List<RawImage>();

        }

        public void OnEnable()
        {
            _startPosition = Vector3.up * (_portraitHeight/2);
            _targetPosition = _startPosition;
        }

        public void FixedUpdate()
        {
            _rect.localPosition = Vector3.Lerp(_rect.localPosition, _targetPosition, _speed);
        }

        #endregion


        #region Mirror

        public override void OnStartClient()
        {
            base.OnStartClient();

            FSM_SetState(_state);
        }

        public void SetAuthority(NetworkConnection conn, int serverPlayerId)
        {
            ServerPlayerId = serverPlayerId;
            Cmd_SetState(CharacterSelectSlotState.Selecting);
            netIdentity.AssignClientAuthority(conn);
        }

        public void RemoveAuthority()
        {
            ServerPlayerId = -1;
            netIdentity.RemoveClientAuthority();
            Cmd_SetState(CharacterSelectSlotState.Closed);            
        }

        #endregion


        public void AddCharacterPortraits()
        {
            foreach (
                CharacterAsset res
                in CharacterLibrary.Instance.Characters)
            {
                if (res == null) continue;

                var portrait =
                    _imageTemplate
                    .Create(_imageTemplate.transform.parent)
                    ?.GetComponent<RawImage>();

                if (portrait != null)
                {
                    if (
                        CharacterRosterPreview
                        .Instance
                        .GetCharacterPreview(
                            ServerPlayerId,
                            res.Id,
                            out CharacterPreview preview))
                    {
                        portrait.texture = preview.RenderTexture;
                    }

                    _portraits.Add(portrait);
                }
            }

            if (_imageTemplate != null)
            {
                _portraitHeight = _portraits[0].GetComponent<LayoutElement>().preferredHeight;
                _totalHeight = _portraitHeight * _portraits.Count;
                _offset = 0;
                _imageTemplate.gameObject.SetActive(false);
            }
        }

        #region FSM

        public void FSM_SetState(CharacterSelectSlotState target)
        {            
            switch (target)
            {                
                case CharacterSelectSlotState.Closed:

                    if (_state == CharacterSelectSlotState.Ready)
                    {
                        GameSession.Instance.CharacterSelectOpenCount =
                            GameSession.Instance.CharacterSelectOpenCount == 0 ?
                            0 :
                            GameSession.Instance.CharacterSelectOpenCount - 1;
                    }

                    _up.gameObject.SetActive(false);
                    _down.gameObject.SetActive(false);
                    _maskRect.gameObject.SetActive(false);
                    _statusText.text = "Press A to join";
                    _statusText.color = _joinColor;
                    break;

                case CharacterSelectSlotState.Selecting:
                    {
                        if (!CharacterRosterPreview
                            .Instance
                            .GetCharacterPreview(
                                ServerPlayerId, 0,
                                out CharacterPreview _))
                        {
                            CharacterRosterPreview
                                .Instance
                                .AddPlayerPreviews(ServerPlayerId);
                            AddCharacterPortraits();
                        }

                        if (_state == CharacterSelectSlotState.Closed)
                        {
                            GameSession.Instance.CharacterSelectOpenCount =
                                GameSession.Instance.CharacterSelectOpenCount >= Controls.PlayerManager.PlayerMax ?
                                    Controls.PlayerManager.PlayerMax :
                                    GameSession.Instance.CharacterSelectOpenCount + 1;
                        }
                        else if (_state == CharacterSelectSlotState.Ready)
                        {
                            GameSession.Instance.CharacterSelectReadyCount--;
                        }

                        if (
                            CharacterRosterPreview
                            .Instance
                            .GetCharacterPreview(
                                ServerPlayerId,
                                CharacterLibrary.Instance.Characters[_selectedIndex].Id,
                                out CharacterPreview preview))
                        {
                            preview.Character.Play(
                                CharacterAnimation.Character_Idle, 
                                true);
                        }

                        //if (_characterSpotlightAnchor.transform.childCount != 0)
                        //    Destroy(_characterSpotlightAnchor.GetChild(0).gameObject);

                        _up.gameObject.SetActive(true);
                        _down.gameObject.SetActive(true);
                        _maskRect.gameObject.SetActive(true);
                        _statusText.text = "";
                    }
                    break;

                case CharacterSelectSlotState.Ready:
                    
                    {
                        if (_state != CharacterSelectSlotState.Ready) GameSession.Instance.CharacterSelectReadyCount++;

                        if (CharacterRosterPreview
                            .Instance
                            .GetCharacterPreview(
                                ServerPlayerId,
                                CharacterLibrary.Instance.Characters[_selectedIndex].Id,
                                out CharacterPreview preview))
                        {
                            preview.Character.Play(CharacterAnimation.Character_Winning, true);
                        }
                    } 

                    _up.gameObject.SetActive(false);
                    _down.gameObject.SetActive(false);
                    //_maskRect.gameObject.SetActive(false);
                    _statusText.text = "Ready";
                    _statusText.color = _readyColor;
                    break;
            }

            _state = target;
        }

        public void Cmd_SetState(CharacterSelectSlotState target)
        {
            CommandClient
                .Instance
                .Cmd_CharacterSelectSlot_SetState(
                    gameObject,
                    target);
        }

        [ClientRpc]
        public void Rpc_SetState(CharacterSelectSlotState target)
        {
            FSM_SetState(target);
        }


        #endregion

        public IEnumerator PunchScale(bool previous)
        {
            iTween.Stop(_up.gameObject);
            iTween.Stop(_down.gameObject);

            _up.transform.localScale = new Vector3(1, 1, 1);
            _down.transform.localScale = new Vector3(1, 1, 1);

            yield return new WaitForSeconds(0.01f);

            if (previous)
            {
                iTween.PunchScale(
                    _up.gameObject,
                    new Vector3(_selectPunchScale,
                    _selectPunchScale, 0),
                    _selectPunchScaleTime);
            }
            else
            {
                iTween.PunchScale(
                    _down.gameObject,
                    new Vector3(_selectPunchScale,
                        _selectPunchScale, 0),
                        _selectPunchScaleTime);
            }
        }
        
        [ClientRpc]
        public void Rpc_Scroll(bool up)
        {
            if (_state != CharacterSelectSlotState.Selecting) return;

            _selectedIndex = up ? _selectedIndex - 1 : _selectedIndex + 1;
            _selectedIndex = Mathf.Clamp(_selectedIndex, 0, CharacterLibrary.Instance.Characters.Length - 1);
            _offset = up ? _offset - _portraitHeight : _offset + _portraitHeight;
            _offset = Mathf.Clamp(_offset, 0, _totalHeight - _portraitHeight);
            _targetPosition = _startPosition + Vector3.up * _offset;

            if (_selectedIndex == 0)
            {
                _up.color = _up.color.SetA(_disabledArrowAlpha);
                if (!up) StartCoroutine(PunchScale(false));
            }
            else if (_selectedIndex == CharacterLibrary.Instance.Characters.Length - 1)
            {
                if (up) StartCoroutine(PunchScale(true));
                _down.color = _down.color.SetA(_disabledArrowAlpha);
            }
            else
            {
                _up.color = _up.color.SetA(1f);
                _down.color = _down.color.SetA(1f);

                if (up) StartCoroutine(PunchScale(true));
                else StartCoroutine(PunchScale(false));
            }
        }

        public void Cmd_Scroll(bool up)
        {            
            if (!hasAuthority) return;

            CommandClient
                .Instance
                .Cmd_CharacterSelectSlot_Scroll(gameObject, up);
        }

        public void HandleAction0()
        {
            switch (_state)
            {
                case CharacterSelectSlotState.Selecting:                    
                    break;

                case CharacterSelectSlotState.Ready:
                    CharacterSelectInterface
                        .Instance
                        .SetState(CharacterSelectInterface.State.Select);
                    Cmd_SetState(CharacterSelectSlotState.Selecting);
                    break;

                case CharacterSelectSlotState.Closed:
                    break;
            }
        }

        public void HandleAction1(params object[] args)
        {
            switch (_state)
            {
                case CharacterSelectSlotState.Selecting:
                    Controls.Player player = (Controls.Player) args[0];
                    //Debug.Log("Assigned id: " + CharacterLibrary.Instance.Characters[_selectedIndex].Id);
                    player._session.CharacterId = CharacterLibrary.Instance.Characters[_selectedIndex].Id;
                    Cmd_SetState(CharacterSelectSlotState.Ready);
                    break;                    

                case CharacterSelectSlotState.Ready:
                    // Try to change the state of the select screen, not the slot
                    CharacterSelectInterface
                        .Instance
                        .SetState(
                            CharacterSelectInterface.State.Ready);
                    break;

                case CharacterSelectSlotState.Closed:
                    Cmd_SetState(CharacterSelectSlotState.Selecting);
                    break;
            }            
        }              
    }
}
