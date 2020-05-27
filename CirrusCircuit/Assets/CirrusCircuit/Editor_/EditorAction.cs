using UnityEngine;
using System.Collections;
using Boo.Lang;
using Cirrus.Circuit.World.Objects;
using System;

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
    public class EditorObjectDescription
    {
        public Vector3Int Position;
        public BaseObject Template;
        public int Rotation;
    }

    [Serializable]
    public class EditorAction
    {
        [SerializeField]
        public Vector3Int Position;
        [SerializeField]
        public List<EditorObjectDescription> Erased = new List<EditorObjectDescription>();
        [SerializeField]
        public List<EditorObjectDescription> Added = new List<EditorObjectDescription>();
        [SerializeField]
        public BaseObject SelectedTile;
        [SerializeField]
        public ActionType Type;

        public bool Equals(EditorAction other)
        {
            if (other == null) return false;

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
