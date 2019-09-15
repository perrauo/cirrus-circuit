using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.Circuit.Objects
{
    public class Podium : MonoBehaviour
    {
        [SerializeField]
        private Platform _platformTemplate;

        [SerializeField]
        private float _platformOffset = 2f;

        [SerializeField]
        private GameObject _platformsParent;

        [SerializeField]
        public Vector3 TargetPosition = Vector3.zero;

        [SerializeField]
        private List<Platform> _platforms;


        [SerializeField]
        private List<Characters.Character> _characters;

        private Timer _timer;

        [SerializeField]
        private float _timeTransitionTo = 2f;

        [SerializeField]
        private float _timeTransitionFrom = 2f;

        [SerializeField]
        public float _positionSpeed = 0.4f;

        bool init = false;

        public void OnEnable()
        {
            if (!init)
            {
                init = true;
                _timer = new Timer(_timeTransitionTo, start: false, repeat: false);
            }
        }

        public void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, TargetPosition, _positionSpeed);

            for(int i = 0; i < _characters.Count; i++) {
                _characters[i].Object.transform.position =
                Vector3.Lerp(
                    _characters[i].Object.transform.position,
                    _platforms[i]._characterAnchor.transform.position,
                    _timer.Time/_timeTransitionFrom);

                _characters[i].Object.transform.rotation = Quaternion.Lerp(
                    _characters[i].Object.transform.rotation, 
                    _platforms[i]._visual.Parent.transform.rotation,
                    _timer.Time / _timeTransitionFrom);


            }
        }

        public void Add(Controls.Controller ctrl, Characters.Character character)
        {
            Platform platform = Instantiate(
                _platformTemplate.gameObject,
                _platformsParent.transform.position + Vector3.right * _platforms.Count * _platformOffset,
                Quaternion.identity,
                _platformsParent.transform).GetComponent<Platform>();

            _characters.Add(character);

            platform.Character = character;
            platform.Controller = ctrl;
            _platforms.Add(platform);

            _timer.OnTimeLimitHandler += platform.OnTransitionToTimeOut;
        }

        public void OnRoundEnd()
        {
            foreach (var p in _platforms)
            {
                if (p == null)
                    continue;

                Destroy(p.gameObject);
            }

            _platforms.Clear();

            _characters.Clear();

            _timer.Start();
        }



    }
}