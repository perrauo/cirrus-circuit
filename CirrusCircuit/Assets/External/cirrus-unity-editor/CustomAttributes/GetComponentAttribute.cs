using UnityEngine;
//using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using UnityEditor;
using System.Linq;



namespace Cirrus.UnityEditorExt
{
    public enum GetComponentAttributeMode
    {
        Self,
        Parent,
        Children
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class GetComponentAttribute : PropertyAttribute
    {
        public Type Type;

        public GetComponentAttributeMode Mode;

        public GetComponentAttribute(Type type, GetComponentAttributeMode mode = GetComponentAttributeMode.Self)
        {
            Type = type;
            Mode = mode;
        }

    }


#if UNITY_EDITOR

    //Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
    //Modified by: Sebastian Lague


    [CustomPropertyDrawer(typeof(GetComponentAttribute))]
    public class GetComponentAttributeDrawer : PropertyDrawer
    {
        public void Default(Rect position, SerializedProperty property, GUIContent label)
        {
            RefEditorGUI.DefaultPropertyField(position, property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        { 
            if (property.objectReferenceValue == null)
            {                
                MonoBehaviour behaviour = property.serializedObject.targetObject as MonoBehaviour;
                if (property.objectReferenceValue == null)
                {
                    GetComponentAttribute att = (GetComponentAttribute)attribute;

                    switch (att.Mode)
                    {
                        case GetComponentAttributeMode.Self:
                            property.objectReferenceValue = behaviour.GetComponent(att.Type);
                            break;

                        case GetComponentAttributeMode.Children:
                            property.objectReferenceValue = behaviour.GetComponentInChildren(att.Type);                                
                            break;

                        case GetComponentAttributeMode.Parent:
                            property.objectReferenceValue = behaviour.GetComponentInParent(att.Type);
                            break;
                    }

                }
            }

            Default(position, property, label);

        }
    }

#endif

}





