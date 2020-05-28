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

        [SerializeField]
        private RenderTexture _texture;

        public bool Create(
            Transform transform,
            int index, 
            out CharacterPreview preview)
        {
            preview = null;

            if (index >= CharacterLibrary.Instance.Characters.Length) return false;

            preview = this.Create(transform);
            preview.transform.localPosition = Vector3.zero;
            preview.gameObject.SetActive(true);
            preview._texture = UILibrary.Instance.CharacterRenderTextures[index];
            preview._camera.targetTexture = preview._texture;
            preview._character = CharacterLibrary.Instance.Characters[index].Create(
                preview._anchor, 
                Quaternion.identity);
            preview._character.InitState(World.Objects.ObjectState.CharacterSelect);
            preview._character.Transform.rotation =
                Quaternion.LookRotation(
                    _camera.transform.position - preview._character.Transform.position,
                    Vector3.up);
            preview._texture = UILibrary.Instance.CharacterRenderTextures[index];            
            preview.transform.localPosition = Vector3.right * index * DistanceBetween;

            return preview;
        }
    }
}