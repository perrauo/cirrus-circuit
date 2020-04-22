using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit.UI
{
    public class LevelSelect : MonoBehaviour
    {
        private bool _enabled = false;

        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                _enabled = value;
                transform.GetChild(0).gameObject.SetActive(_enabled);
            }
        }

        [SerializeField]
        private float _selectPunchScale = 0.5f;

        [SerializeField]
        private float _selectPunchScaleTime = 1f;


        [SerializeField]
        private UnityEngine.UI.Text _levelName;

        [SerializeField]
        private UnityEngine.UI.Text _previous;

        [SerializeField]
        private UnityEngine.UI.Text _next;

        public void OnValidate()
        {
        }


        public void Awake()
        {
            Game.Instance.OnLevelSelectedHandler += OnLevelSelected;
            Game.Instance.OnLevelSelectHandler += OnLevelSelect;
        }

        public void OnDestroy()
        {
            if (Game.Instance.OnLevelSelectedHandler != null)
            {
                Game.Instance.OnLevelSelectedHandler -= OnLevelSelected;
                Game.Instance.OnLevelSelectHandler -= OnLevelSelect;
            }
        }

        public IEnumerator PunchScale(bool previous)
        {
            iTween.Stop(_previous.gameObject);
            iTween.Stop(_next.gameObject);

            _previous.transform.localScale = new Vector3(1, 1, 1);
            _next.transform.localScale = new Vector3(1, 1, 1);

            yield return new WaitForSeconds(0.01f);

            if (previous)
            {

                iTween.PunchScale(
                    _previous.gameObject,
                    new Vector3(_selectPunchScale,
                    _selectPunchScale, 0),
                    _selectPunchScaleTime);
            }
            else
            {
                iTween.PunchScale(
                    _next.gameObject,
                    new Vector3(_selectPunchScale,
                    _selectPunchScale, 0),
                    _selectPunchScaleTime);
            }
        }


        public void OnLevelSelect(bool enabled)
        {
            Enabled = enabled;
        }


        public void OnLevelSelected(World.Level level, int step)
        {
            if (Game.Instance._currentLevelIndex == 0)
            {
                _previous.gameObject.SetActive(false);
            }
            else
            {
                _previous.gameObject.SetActive(true);
            }

            if (Game.Instance._currentLevelIndex == Game.Instance._levels.Length - 1)
            {
                _next.gameObject.SetActive(false);
            }
            else
            {
                _next.gameObject.SetActive(true);
            }

            if (step < 0)
            {
                StartCoroutine(PunchScale(true));
            }
            else if (step > 0)
            {
                StartCoroutine(PunchScale(false));
            }

            if (Game.Instance._selectedLevel != null)
                _levelName.text = Game.Instance._selectedLevel.Name;

            // TODO upd num of players ??
            //foreach (var display in _playerDisplays)
            //{
            //    display.TryChangeState(Player.State.Disabled);
            //}
        }

    }
}
