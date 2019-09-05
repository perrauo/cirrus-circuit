using System;
using System.Collections;
using System.Collections.Generic;
using Cirrus.GemCircuit.Objects.Characters;
using UnityEngine;

namespace Cirrus.GemCircuit.Objects
{
    // TODO remove


    public class PlayerSpawn : BaseObject
    {
        [SerializeField]
        private GameObject _characterTemplate;

        private Color _color;

        public Color Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
                _visual.Color = _color;
            }
        }

        public override bool TryMove(Vector3 step, BaseObject incoming = null)
        {
            return false;           
        }

        protected override void Awake()
        {
            base.Awake();         

            //_visual.gameObject.SetActive(false);
            _visual.Enabled = false;
        }

        public override bool Accept(BaseObject incoming)
        {
            //incoming.Fall();
            return true;
        }

        public Character Spawn()
        {
            var character = Instantiate(_characterTemplate, transform.position, Quaternion.identity).GetComponentInChildren<Character>();
            character.Color = _color;
            return character;
        }
    }
}
