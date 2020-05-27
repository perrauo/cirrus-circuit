using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Cirrus
{
#if UNITY_EDITOR

    /**
	 *	An editor utility for easily creating symlinks in your project.
	 *
	 *	Adds a Menu item under `Assets/Create/Folder (Symlink)`, and 
	 *	draws a small indicator in the Project view for folders that are
	 *	symlinks.
	 */
    [InitializeOnLoad]
    public static class SymlinkUtils
    {
        // FileAttributes that match a junction folder.
        const FileAttributes FOLDER_SYMLINK_ATTRIBS = FileAttributes.Directory | FileAttributes.ReparsePoint;

        // Style used to draw the symlink indicator in the project view.
        private static GUIStyle _symlinkMarkerStyle = null;
        private static GUIStyle symlinkMarkerStyle
        {
            get
            {
                if (_symlinkMarkerStyle == null)
                {
                    _symlinkMarkerStyle = new GUIStyle(EditorStyles.label);
                    _symlinkMarkerStyle.normal.textColor = new Color(.2f, .8f, .2f, .8f);
                    _symlinkMarkerStyle.alignment = TextAnchor.MiddleRight;
                }
                return _symlinkMarkerStyle;
            }
        }

        /**
		 *	Static constructor subscribes to projectWindowItemOnGUI delegate.
		 */
        static SymlinkUtils()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        /**
		 *	Draw a little indicator if folder is a symlink
		 */
        private static void OnProjectWindowItemGUI(string guid, Rect r)
        {
            try
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (!string.IsNullOrEmpty(path))
                {
                    FileAttributes attribs = File.GetAttributes(path);

                    if ((attribs & FOLDER_SYMLINK_ATTRIBS) == FOLDER_SYMLINK_ATTRIBS)
                        GUI.Label(r, "<=>", symlinkMarkerStyle);
                }
            }
            catch { }
        }


        /**
 *	Add a menu item in the Assets/Create category to add symlinks to directories.
 */
        public static string GetAssetPath()
        {
            string path;

            path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            return path;
        }



        [DllImport("kernel32.dll")]
        static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }

        //static void Main(string[] args)
        //{
        //    string symbolicLink = @"c:\bar.txt";
        //    string fileName = @"c:\temp\foo.txt";

        //    using (var writer = File.CreateText(fileName))
        //    {
        //        writer.WriteLine("Hello World");
        //    }

        //    CreateSymbolicLink(symbolicLink, fileName, SymbolicLink.File);
        //}

        [MenuItem("Assets/Create File Symlink", false, 20)]
        static void DoFileSymlink()
        {
            foreach (Object o in Selection.objects)
            {
                string projpath = Path.GetDirectoryName(Application.dataPath);

                string targetPath = projpath + "\\"+ AssetDatabase.GetAssetPath(o);

                string destFileext = Path.GetExtension(targetPath);

                string destFileName = Path.GetFileNameWithoutExtension(targetPath);

                string destFolderName = Path.GetDirectoryName(targetPath);

                UnityEngine.Debug.Log($"{destFolderName}\\{destFileName}.symlink{destFileext}");

                CreateSymbolicLink($"{destFolderName}\\{destFileName}.symlink{destFileext}", targetPath, SymbolicLink.File);

                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                return;
            }

        }


        /**
		 *	Add a menu item in the Assets/Create category to add symlinks to directories.
		 */
        [MenuItem("Assets/Create Folder Symlink", false, 20)]
        static void DoTheSymlink()
        {
            string sourceFolderPath = EditorUtility.OpenFolderPanel("Select Folder Source", "", "");

            if (sourceFolderPath.Contains(Application.dataPath))
            {
                UnityEngine.Debug.LogWarning("Cannot create a symlink to folder in your project!");
                return;
            }

            // Cancelled dialog
            if (string.IsNullOrEmpty(sourceFolderPath))
                return;

            string sourceFolderName = sourceFolderPath.Split(new char[] { '/', '\\' }).LastOrDefault();

            if (string.IsNullOrEmpty(sourceFolderName))
            {
                UnityEngine.Debug.LogWarning("Couldn't deduce the folder name?");
                return;
            }

            Object uobject = Selection.activeObject;

            string targetPath = uobject != null ? AssetDatabase.GetAssetPath(uobject) : "";

            if (string.IsNullOrEmpty(targetPath))
                targetPath = "Assets";

            FileAttributes attribs = File.GetAttributes(targetPath);

            if ((attribs & FileAttributes.Directory) != FileAttributes.Directory)
                targetPath = Path.GetDirectoryName(targetPath);

            targetPath = string.Format("{0}/{1}", targetPath, sourceFolderName);

            if (Directory.Exists(targetPath))
            {
                UnityEngine.Debug.LogWarning(string.Format("A folder already exists at this location, aborting link.\n{0} -> {1}", sourceFolderPath, targetPath));
                return;
            }

#if UNITY_EDITOR_WIN
            Process cmd = Process.Start("CMD.exe", string.Format("/C mklink /J \"{0}\" \"{1}\"", targetPath, sourceFolderPath));
            cmd.WaitForExit();
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
			// @todo
#endif

            // UnityEngine.Debug.Log(string.Format("Created symlink: {0} <=> {1}", targetPath, sourceFolderPath));

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }

#endif
}
