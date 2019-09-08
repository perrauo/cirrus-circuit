using Cirrus.Circuit.Controls;
using Cirrus.Circuit.Objects.Characters;
using Cirrus.Circuit.Objects.Characters.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace Cirrus.Circuit
{
    public class Layers
    {
        //public int 
        public int MoveableFlags = 1 << LayerMask.NameToLayer("Moveable");
        public int SolidFlags = 1 << LayerMask.NameToLayer("Solid");
        public int Moveable = LayerMask.NameToLayer("Moveable");
        public int Solid = LayerMask.NameToLayer("Solid");
    }

    public class Game : MonoBehaviour
    {

        private static Game _instance;

        public static Game Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<Game>(); ;
                }

                return _instance;

            }
        }


        [SerializeField]
        public Clock _clock;


        public Clock Clock
        {
            get
            {
                return _clock;
            }
        }

        [SerializeField]
        public Lobby Lobby;

        [SerializeField]
        public UI.HUD HUD;


        public FSM.State State {
            get {
                return _stateMachine._state;
            }
        }


        public Layers Layers;

        [SerializeField]
        public Levels.Level[] _levels;

        public Levels.Level CurrentLevel;

        public int _currentLevelIndex = 0;

        public float _distanceLevelSelect = 35;


        [SerializeField]
        private FSM _stateMachine;

        [SerializeField]
        public Camera  _camera;

        [SerializeField]
        public float _targetSizeCamera = 10f;

        [SerializeField]
        public float _cameraSizeSpeed = 0.8f;


        void Awake()
        {
            //if (_instance != null)
            //{
            //    Destroy(gameObject);
            //    return;
            //}

            Layers = new Layers();
            DontDestroyOnLoad(this.gameObject);             
        }

        public void Start()
        {
     
            
        }

        public void OnValidate()
        {
            _levels = GetComponentsInChildren<Levels.Level>(true);
            CurrentLevel = _levels.Length == 0 ? null : _levels[0];
        }


        public void OnLevelSelected(int step)
        {
            for (int i = 0; i < _levels.Length; i++)
            {
                if (_levels[i] == null)
                    continue;

                _levels[i].TargetPosition = Vector3.zero + Vector3.right * (i - _currentLevelIndex) * _distanceLevelSelect;

            }

            CurrentLevel = _levels[_currentLevelIndex];

            _targetSizeCamera = CurrentLevel.CameraSize;

            HUD.OnLevelSelected(step);

        }


        // TODO: Simulate LeftStick continuous axis with WASD
        public void HandleAxesLeft(Controller controller, Vector2 axis)
        {
            _stateMachine.HandleAxesLeft(controller, axis);
        }


        public void HandleAction0(Controller controller)
        {
            _stateMachine.HandleAction0(controller);
        }

        public void HandleAction1(Controller controller)
        {
            _stateMachine.HandleAction1(controller);
        }

    }
}
