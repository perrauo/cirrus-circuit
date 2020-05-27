#if UNITY_EDITOR

using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Cirrus.UnityEditorExt
{
    public class Utils
    {
        [MenuItem("Assets/Create .txt", false, 1)]
        static void CreateTxt()
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(Selection.activeObject);

            string unique = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + "/Note.txt");

            // Debug.Log(unique);

            using (StreamWriter writer = new StreamWriter(unique, false))
            {
                writer.WriteLine("Hello world..");
                writer.Flush();
            }

            UnityEditor.AssetDatabase.Refresh();

        }

        [MenuItem("Assets/Create .json", false, 1)]
        static void CreateJson()
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(Selection.activeObject);

            string unique = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + "/Document.json");

            // Debug.Log(unique);

            using (StreamWriter writer = new StreamWriter(unique, false))
            {
                writer.WriteLine("{}");
                writer.Flush();
            }

            UnityEditor.AssetDatabase.Refresh();

        }

        [MenuItem("Assets/Create .cs", false, 1)]
        static void CreateCs()
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(Selection.activeObject);

            string unique = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + "/MyClass.cs");

            Debug.Log(unique);

            using (StreamWriter writer = new StreamWriter(unique, false))
            {
                writer.WriteLine("");
                //writer.WriteLine("aInt.ToString() to write");
                writer.Flush();
            }

            UnityEditor.AssetDatabase.Refresh();

        }
    }
}


#endif