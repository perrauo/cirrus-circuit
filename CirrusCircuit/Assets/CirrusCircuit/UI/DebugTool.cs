using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit
{
    public class DebugTool : MonoBehaviour
    {
        [SerializeField]
        private Controls.LocalPlayerManager _lobby;

        public void OnValidate()
        {
            if (_lobby == null)
                _lobby = FindObjectOfType<Controls.LocalPlayerManager>();

        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.P))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    _lobby.Players[0].Score += 10f;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    _lobby.Players[1].Score += 10f;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    _lobby.Players[2].Score += 10f;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    _lobby.Players[3].Score += 10f;
                }
            }
        }

    }
}
