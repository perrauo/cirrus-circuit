using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit.UI
{
    public class CharacterSelect : MonoBehaviour
    {
        [SerializeField]
        private CharacterSelectSlot[] slots;

        public void OnValidate()
        {
            if(slots.Length == 0)
                slots = GetComponentsInChildren<CharacterSelectSlot>();
        }
    }
}
