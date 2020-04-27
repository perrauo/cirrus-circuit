using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit
{
    public class DebugTool : MonoBehaviour
    {
        [SerializeField]
        private Controls.PlayerManager _lobby;

        public void OnValidate()
        {
            if (_lobby == null)
                _lobby = FindObjectOfType<Controls.PlayerManager>();

        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.P))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    //_lobby.LocalPlayers[0].Score += 10f;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    //_lobby.LocalPlayers[1].Score += 10f;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    //_lobby.LocalPlayers[2].Score += 10f;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    //_lobby.LocalPlayers[3].Score += 10f;
                }
            }
        }

    }
}
