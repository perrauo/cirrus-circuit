using Cirrus.Circuit.Controls;
using Cirrus.Circuit.UI;
using Cirrus;
using Mirror;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using Cirrus.MirrorExt;
using Cirrus.Circuit.World;

namespace Cirrus.Circuit.World.Objects
{

    public class Spawner : MonoBehaviour
    {
        [Header("----------------------------", order = 0)]
        [Header("Spawner", order = 1)]
        [Header("----------------------------", order = 2)]
        [SerializeField]
        private Spawnable[] _spawnables;

        public bool _isRespawn;

        [UnityEditorExt.EnumFlag]
        public MoveableObjectType _respawnObjectFilter = (MoveableObjectType)0xFFFFFFF;
        
        [UnityEditorExt.EnumFlag]
        public Flag _respawnIdFilter = (Flag)0xFFFFFFF;

        public float _respawnDelay = 2f;

        public Numeric.Range _spawnTime = new Numeric.Range(4, 4);

        //TODO
        private bool _isAnimated = true;




        // Use this for initialization
        public virtual void Start()
        {

        }

        // Update is called once per frame
        public virtual void Update()
        {

        }
    }
}