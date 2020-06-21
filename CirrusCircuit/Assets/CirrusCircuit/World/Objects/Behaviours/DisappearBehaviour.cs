using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using System.Threading.Tasks;
using Cirrus.Circuit.Networking;

namespace Cirrus.Circuit.World.Objects
{
    public class DisappearBehaviour : MonoBehaviour
    {
        [SerializeField]
        private BaseObject _target;

        private Timer _timer;

        [SerializeField]
        private float _disappearTimeInterval = 2f;

        private bool _isDisappeared = false;

        public void Awake()
        {
            if (CustomNetworkManager.IsServer)
            {
                _timer = new Timer(_disappearTimeInterval, false, repeat: true);
                _timer.OnTimeLimitHandler += OnTimeout;
            }
        }

        public void Start()
        {
            if (CustomNetworkManager.IsServer)
            {
                _timer.Start();
            }
        }

        public void OnTimeout()
        {
            if (LevelSession.Instance.GetMoveResults(
                new Move
                {
                    User = _target,
                    Type = _isDisappeared ? MoveType.Reappear : MoveType.Disappear,
                    Position = _target._levelPosition,
                    Destination = _target._levelPosition,
                    Step = Vector3Int.zero,
                    //Destination _target._levelPosition,
                    Entered = null,
                    Source = null,
                }, out IEnumerable<MoveResult> results)
                > 0)
            {
                LevelSession.Instance.ApplyMoveResults(results);
            }

            //CommandClient
            //    .Instance
            //    .Cmd_ObjectSession_PerformAction(
            //        gameObject,
            //        _isDisappeared ? ObjectAction.Reappear : ObjectAction.Disappear);                   
        }
    }
}