using UnityEngine;
using System.Collections;

namespace Cirrus.Extensions
{

    public static class Vector3Extension
    {
        public static Vector3 X0Y(this Vector2 vector)
        {
            return new Vector3(vector.x, 0, vector.y);
        }

        public static Vector3 X0Z(this Vector3 vector)
        {
            return new Vector3(vector.x, 0, vector.z);
        }


        public static bool CloseEnough(this Vector3 pos0, Vector3 pos1, float tolerance = 0.1f)
        {
            return (Vector3.Distance(pos0, pos1) <= tolerance);
        }


        /// <summary>
        /// Rounds Vector3.
        /// </summary>
        /// <param name="vector3"></param>
        /// <param name="decimalPlaces"></param>
        /// <returns></returns>
        public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
        {
            float multiplier = 1;
            for (int i = 0; i < decimalPlaces; i++)
            {
                multiplier *= 10f;
            }

            return new Vector3(
                Mathf.Round(vector3.x * multiplier) / multiplier,
                Mathf.Round(vector3.y * multiplier) / multiplier,
                Mathf.Round(vector3.z * multiplier) / multiplier);
        }

        public static Vector3 ToVector3(this Vector3Int vec3)
        {
            return new Vector3(vec3.x, vec3.y, vec3.z);
        }

        public static Vector3Int ToVector3Int(this Vector3 vec3)
        {
            return new Vector3Int((int)vec3.x, (int)vec3.y, (int)vec3.z);
        }

        public static Vector2Int ToVector2Int(this Vector2 vec2)
        {
            return new Vector2Int((int)vec2.x, (int)vec2.y);// (int)vec3.z);
        }
    }
}
