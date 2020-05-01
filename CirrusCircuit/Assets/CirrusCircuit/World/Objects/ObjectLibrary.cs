using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cirrus.Circuit.World.Objects
{
    public class ObjectLibrary : Resources.BaseAssetLibrary<ObjectLibrary>
    {    
        [SerializeField]
        public List<Spawnable> Objects;

        public Gem[] Gems;

        public Spawnable Get(int id)
        {
            return Objects[id];
        }

        public bool TryGet(int id, out Spawnable spawnable)
        {
            spawnable = null;
            if (id < 0) return false;
            if (id > Objects.Count) return false;
            if (Objects[id] == null) return false;
            spawnable = Objects[id];
            return true;
        }

        public void SortId()
        {
            foreach (var obj in Objects)
            {
                obj._id = Objects.IndexOf(obj);
            }
        }

#if UNITY_EDITOR
        public void UpdateSpawnableList()
        {
            if (Instance != null)
            {
                Objects.Clear();                

                foreach (FieldInfo field in typeof(ObjectLibrary).GetFields())
                {                   
                    var val = field.GetValue(Instance);
                    if (val != null)
                    {                 

                        if (val is MonoBehaviour)
                        {
                            var mono = (MonoBehaviour)val;

                            if (mono.TryGetComponent(out Spawnable spawn))
                            {
                                Objects.Add(spawn);
                            }
                        }
                        else if (val.GetType().IsArray)
                        {
                            if (field.Name == "Objects") continue;

                            foreach (var arval in (Array)val)
                            {
                                if (arval is MonoBehaviour)
                                {
                                    var mono = (MonoBehaviour)arval;

                                    if (mono.TryGetComponent(out Spawnable spawn))
                                    {
                                        Objects.Add(spawn);
                                    }
                                }
                                else if (arval is GameObject)
                                {
                                    var mono = (GameObject)arval;

                                    if (mono.TryGetComponent(out Spawnable spawn))
                                    {
                                        Objects.Add(spawn);
                                    }
                                }
                            }
                        }
                        else if (val is GameObject)
                        {
                            var mono = (GameObject)val;

                            if (mono.TryGetComponent(out Spawnable spawn))
                            {
                                Objects.Add(spawn);
                            }
                        }

                    }

                }

                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(this);
            }
        }
#endif

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ObjectLibrary))]
    public class ObjectLibraryEditor : UnityEditor.Editor
    {
        private ObjectLibrary _man;

        public virtual void OnEnable()
        {
            _man = serializedObject.targetObject as ObjectLibrary;

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

            if (GUILayout.Button("Update spawnable list"))
            {
                _man.UpdateSpawnableList();
            }
        }
    }

#endif
}