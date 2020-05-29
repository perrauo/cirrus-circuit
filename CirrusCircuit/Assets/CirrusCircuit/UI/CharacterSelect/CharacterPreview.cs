using Cirrus.Circuit.World.Objects.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cirrus.Circuit.UI
{
    public class CharacterPreview : MonoBehaviour
    {
        public const float DistanceBetween = 10f;

        [SerializeField]
        private Transform _anchor;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private Character _character;

        private CharacterAsset _characterAsset;

        private RenderTexture _renderTexture;
        public RenderTexture RenderTexture => _renderTexture;


        public void UpdateCamera()
        {
            _camera.fieldOfView = _characterAsset.Preview_FOV;
            _camera.transform.position = transform.position;
            _camera.transform.position += Vector3.forward * _characterAsset.Preview_OffsetZ;
            _camera.transform.position += Vector3.up * _characterAsset.Preview_OffsetY;
            _camera.transform.rotation = _characterAsset.Preview_IsLookAtEnabled ?
                Quaternion.LookRotation(
                    _character.Transform.position - _camera.transform.position,
                    Vector3.up) :
                Quaternion.Euler(
                    _characterAsset.Preview_PitchAngle, 
                    _characterAsset.Preview_YawAngle, 
                    0);

        }

        public void Init(
            CharacterAsset asset,
            RenderTexture renderTexture,
            float horizontalOffset
            )            
        {
            _characterAsset = asset;
            _renderTexture = renderTexture;
            _camera.targetTexture = _renderTexture;

            transform.localPosition = Vector3.right * horizontalOffset;

            _character = asset.Create(
                transform.position,
                _anchor,
                Quaternion.identity);
            _character.Transform.rotation =
                Quaternion.LookRotation(
                    Vector3.forward,
                    Vector3.up);

                      
            _character.FSM_SetState(World.Objects.ObjectState.CharacterSelect);
            UpdateCamera();

        }


        public void Update()
        {
#if UNITY_EDITOR
            UpdateCamera();
#endif
        }


        public CharacterPreview Create(
            Transform transform,
            int index)
        {
            int characterIndex = index % CharacterLibrary.Instance.Characters.Length;

            CharacterPreview preview = this.Create(transform);
            preview.gameObject.SetActive(true);            
            preview.Init(
                CharacterLibrary.Instance.Characters[characterIndex],
                UILibrary.Instance.CharacterRenderTextures[index],
                index * DistanceBetween
                );

            return preview;
        }
    }
}