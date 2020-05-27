using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using System.Linq;

namespace Cirrus.UnityEditorExt
{
    public static class AssetDatabaseUtils
    {

        public static T FindObjectOfType<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        public static Object FindObjectOfType(System.Type type)
        {
            string[] guids = AssetDatabase.FindAssets("t:" + type);  //FindAssets uses tags check documentation for more info
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return AssetDatabase.LoadAssetAtPath(path, type);
        }

        public static T[] FindObjectsOfType<T>() where T : Object
        {
            List<string> guids = AssetDatabase.FindAssets("t:" + typeof(T)).ToList();
            var prefabs = AssetDatabase.FindAssets("t:Prefab");
            foreach (var prefab in prefabs)
            {
                if(!guids.Contains(prefab)) guids.AddRange(prefabs);
            }

            List<T> assets = new List<T>();
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                assets.Add(AssetDatabase.LoadAssetAtPath<T>(path));
            }

            return assets.ToArray();
        }
    }
}
