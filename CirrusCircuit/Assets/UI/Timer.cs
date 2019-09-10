using UnityEngine;
using System.Collections;
using System;

namespace Cirrus.Circuit.UI
{

    public class Timer : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Text _text;

        public float Time
        {
            set
            {
                var span = new TimeSpan(0, 0, (int)value); //Or TimeSpan.FromSeconds(seconds); (see Jakob C´s answer)
                _text.text = string.Format(span.ToString(@"mm\:ss"));
            }

        }

    }
}
