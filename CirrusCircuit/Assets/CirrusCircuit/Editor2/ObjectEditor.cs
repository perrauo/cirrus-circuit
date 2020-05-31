using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using System.Collections;
using Cirrus;
using System;
using UnityEditor;
using Cirrus.Circuit.World.Objects;

namespace Cirrus.Circuit.Editor2
{
    public class ObjectEditor : MonoBehaviour
    {
        [SerializeField]
        private World.Objects.BaseObject _object;
        public World.Objects.BaseObject Object => _object;

        public void OnValidate()
        {
            if (_object == null) _object = GetComponent<BaseObject>();
        }
    }

    //#if UNITY_EDITOR
    [CustomEditor(typeof(ObjectEditor))]
    public class ObjectEditorCustom : UnityEditor.Editor
    {
        private ObjectEditor _editor;

        public virtual void OnEnable()
        {
            _editor = serializedObject.targetObject as ObjectEditor;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Rotate--"))
            {
                _editor.Object.RotationIndex--;
                EditorUtility.SetDirty(_editor.Object.gameObject);
            }

            if (GUILayout.Button("Rotate++"))
            {
                _editor.Object.RotationIndex++;
                EditorUtility.SetDirty(_editor.Object.gameObject);
            }
        }
    }

}
