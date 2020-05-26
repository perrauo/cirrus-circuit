using UnityEngine;
using System.Collections;
using Boo.Lang;
using Cirrus.Circuit.World.Objects;

namespace Cirrus.Circuit.Editor
{
    public enum ActionType
    { 
        Unknown,
        Erase,
        Draw,
        Move,
        Fill
    }

    [System.Serializable]
    public class EditorAction
    {
        [SerializeField]
        public Vector3Int Position;
        [SerializeField]
        public List<BaseObject> Erased = new List<BaseObject>();
        [SerializeField]
        public List<BaseObject> Added = new List<BaseObject>();
        [SerializeField]
        public BaseObject SelectedTile;
        [SerializeField]
        public ActionType Type;

        public bool Equals(EditorAction other)
        {
            switch (Type)
            {
                case ActionType.Draw:
                    if (Position != other.Position) return false;
                    if (SelectedTile != other.SelectedTile) return false;
                    if (Type != other.Type) return false;
                    return true;

                case ActionType.Erase:
                    if (Position != other.Position) return false;                    
                    if (Type != other.Type) return false;
                    return true;


                default:return false;
            }



        }

    }


}
