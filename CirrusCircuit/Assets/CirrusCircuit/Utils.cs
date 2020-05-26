using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit
{
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public static class CircuitUtils
    {
        public static float IndexToAngle(this int idx)
        {

            switch (idx)
            {
                case 0: return 0f;
                case 1: return 90f;
                case 2: return 180f;
                case 3: return 270f;
                default: return 0f;
            }            
        }


        public static int DirectionToIndex(this Vector3Int direction)
        {
            if (direction == new Vector3Int(0, 0, 1))
            {
                return 0;
            }
            else if (direction == new Vector3Int(-1, 0, 0))
            {
                return 1;
            }
            else if (direction == new Vector3Int(0, 0, 1))
            {
                return 2;
            }
            else if (direction == new Vector3Int(1, 0, 0))
            {
                return 3;
            }
            else return -1;
        }
    }
}