using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit.UI
{

    public class CountDown : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Text _text;

        private int _value = 3;

        private string _message = "Go!";

        public int Number
        {
            get
            {
                return _value;
            }

            set
            {

                _value = value;

                if (_value == 0)
                {
                    _text.gameObject.SetActive(true);
                    _value = 0;
                    _text.text = _message;
                    return;
                }
                else if(_value < 0)
                {
                    _text.gameObject.SetActive(false);
                    return;
                }

                _text.gameObject.SetActive(true);
                _text.text = _value.ToString();
            }
        }



    }
}
