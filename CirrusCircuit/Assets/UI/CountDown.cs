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
                if (_value <= 0)
                {
                    _value = 0;
                    _text.text = _message;
                    return;
                }

                _text.text = _value.ToString();
                _value = value;
            }
        }



    }
}
