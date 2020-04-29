using UnityEngine;
using System.Collections;
using Cirrus.Circuit.Networking;
using Cirrus.Circuit.World;

namespace Cirrus.Circuit.UI
{
    public class LevelSelect : MonoBehaviour
    {
        private bool _enabled = false;

        public bool Enabled
        {
            get => _enabled;

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
            GameSession.OnStartClientStaticHandler += OnClientStarted;
            Game.Instance.OnLevelSelectHandler += OnLevelSelect;

            Game.Instance.OnLevelSelectedHandler += OnLevelSelected;
            Game.Instance.OnLevelScrollHandler += OnLevelScroll;
        }

        public void OnClientStarted(bool enable)
        {

        }

        public void OnDestroy()
        {

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

        public void OnLevelScroll(Level level, int step)
        {
            StartCoroutine(PunchScale(step < 0));
        }

        public void OnLevelSelected(Level level, int index)
        {
            _previous.gameObject.SetActive(
                Game.Instance.SelectedLevelIndex != 0);
            _next.gameObject.SetActive(
                Game.Instance.SelectedLevelIndex != Game.Instance._levels.Length - 1);           

            _levelName.text = level.Name;

            // TODO upd num of players ??
            //foreach (var display in _playerDisplays)
            //{
            //    display.TryChangeState(Player.State.Disabled);
            //}
        }

    }
}
