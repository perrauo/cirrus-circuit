using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace Cirrus.Circuit.World.Objects.Characters
{
    public class CharacterLibrary : Resources.BaseAssetLibrary<CharacterLibrary>
    {
        [SerializeField]
        public CharacterAsset[] Characters;


#if UNITY_EDITOR
        public void SortId()
        {
            foreach (var chara in Characters)
            {
                chara._id = Array.IndexOf(Instance.Characters, chara);
                EditorUtility.SetDirty(chara);

            }
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CharacterLibrary))]
    public class CharacterLibraryEditor : UnityEditor.Editor
    {
        private CharacterLibrary _man;

        public virtual void OnEnable()
        {
            _man = serializedObject.targetObject as CharacterLibrary;

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Called whenever the inspector is drawn for this object.
            //DrawDefaultInspector();

            if (GUILayout.Button("Sort Ids"))
            {
                _man.SortId();
            }
        }
    }

#endif

}