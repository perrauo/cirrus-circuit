using UnityEngine;
using System.Collections;
using Cirrus.Utils;
using System;

namespace Cirrus.Circuit
{
    public class Visual : MonoBehaviour
    {
        //[SerializeField]
        //private World.Resources _resources;

        [SerializeField]
        public GameObject Parent;

        [SerializeField]
        private MeshRenderer[] _meshRenderers;

        [SerializeField]
        private SpriteRenderer[] _spriteRenderers;

        [SerializeField]
        private UnityEngine.UI.Image[] _images;

        [SerializeField]
        private UnityEngine.UI.Text[] _texts;

        [SerializeField]
        private UnityEngine.UI.Outline[] _outlines;

        [SerializeField]
        private Cirrus.UI.ProgressBars.BarViewColor[] _barcolors;

        private Color _color;

        private bool _enabled;

        public void OnValidate()
        {
            //if (_resources == null)
            //    _resources = Utils.AssetDatabase.FindObjectOfType<World.Resources>();
        }

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

        public void MakeUnique()
        {
            foreach (var rend in _meshRenderers)
            {
                if (rend == null)
                    continue;

                if (rend.sharedMaterial == null)
                    continue;

                rend.sharedMaterial = new Material(rend.sharedMaterial);//.color.Set(_color.r, _color.g, _color.b);
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

                //if (_meshRenderers.Length != 0)
                //{
                //    if (_meshRenderers[0].sharedMaterial != null)
                //    {
                //        mat = new Material(_meshRenderers[0].sharedMaterial);
                //        mat.color = _color;//.red;
                //    }
                //}

                foreach (var rend in _meshRenderers)
                {
                    if (rend == null)
                        continue;

                    if (rend.sharedMaterial == null)
                        continue;

                    rend.sharedMaterial.color = rend.sharedMaterial.color.Set(_color.r, _color.g, _color.b);
                }

                foreach (var rend in _spriteRenderers)
                {
                    if (rend == null)
                        continue;

                        rend.color = rend.color.Set(_color.r, _color.g, _color.b);
                }

                foreach (var im in _images)
                {
                    if (im == null)
                        continue;

                    im.color = im.color.Set(_color.r, _color.g, _color.b);
                }
           
                foreach (var o in _outlines)
                {
                    if (o == null)
                        continue;

                    o.effectColor = o.effectColor.Set(_color.r, _color.g, _color.b);
                }

                foreach (var o in _texts)
                {
                    if (o == null)
                        continue;

                    o.color = o.color.Set(_color.r, _color.g, _color.b);
                }

                foreach (var o in _barcolors)
                {
                    if (o == null)
                        continue;

                    o.SetBarColor(_color);// = _color;
                }
            }
        }

    }
}
