using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cirrus.UnityEditorExt;

using System.Threading.Tasks;
using Cirrus.Circuit.Networking;

namespace Cirrus.Circuit.World.Objects
{
    public class ChangeColorBehaviour : MonoBehaviour
    {
        [SerializeField]
        [GetComponent(typeof(BaseObject))]
        private BaseObject _target;

        private Timer _timer;

        [SerializeField]
        private float _disappearTimeInterval = 2f;

        //private bool _colorID = false;

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
            //{
            //    //LevelSession.Instance.Apply(results);
            //}


            _target.Cmd_Perform(new Action
            {
                Type = ActionType.Color,
                ColorID = MathUtils.Wrap(
                    _target.ColorID,
                    0,
                    GameSession.Instance.PlayerCount)
            });

            //CommandClient
            //    .Instance
            //    .Cmd_ObjectSession_PerformAction(
            //        gameObject,
            //        _isDisappeared ? ObjectAction.Reappear : ObjectAction.Disappear);                   
        }
    }
}