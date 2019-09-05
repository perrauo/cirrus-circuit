using UnityEngine;
using System.Collections;

namespace Cirrus.Gembalaya
{
    public class Visual : MonoBehaviour
    {
        [SerializeField]
        public GameObject Parent;

        [SerializeField]
        private MeshRenderer[] _meshRenderers;

        [SerializeField]
        private SpriteRenderer[] _spriteRenderers;

        [SerializeField]
        private UnityEngine.UI.Image[] _images;

        private Color _color;

        private bool _enabled;

        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                _enabled = value;

                foreach (var rend in _meshRenderers)
                {
                    if (rend == null)
                        continue;

                    rend.enabled = _enabled;
                }

                foreach (var rend in _spriteRenderers)
                {
                    if (rend == null)
                        continue;

                    rend.enabled = _enabled;
                }

            }
        }

        public Color Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
                Material mat = null;

                if (_meshRenderers.Length != 0)
                {          
                    mat = new Material(_meshRenderers[0].sharedMaterial);
                    mat.color = _color;//.red;
                }

                foreach (var rend in _meshRenderers)
                {
                    if (rend == null)
                        continue;

                    rend.sharedMaterial = mat;
                }

                foreach (var rend in _spriteRenderers)
                {
                    if (rend == null)
                        continue;

                        rend.color = _color;
                }

                foreach (var im in _images)
                {
                    if (im == null)
                        continue;

                    im.color = _color;
                }
            }
        }

    }
}
