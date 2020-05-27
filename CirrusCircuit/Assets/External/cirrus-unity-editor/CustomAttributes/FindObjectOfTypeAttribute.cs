using UnityEngine;
//using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using UnityEditor;
using System.Linq;



namespace Cirrus.UnityEditorExt
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FindObjectOfTypeAttribute : PropertyAttribute
    {
        public Type Type;

        public FindObjectOfTypeAttribute(Type type)
        {
            Type = type; 
        }

    }


#if UNITY_EDITOR

    //Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
    //Modified by: Sebastian Lague


    [CustomPropertyDrawer(typeof(FindObjectOfTypeAttribute))]
    public class FindObjectOfTypePropertyDrawer : PropertyDrawer
    {
        public void Default(Rect position, SerializedProperty property, GUIContent label)
        {
            RefEditorGUI.DefaultPropertyField(position, property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        { 
            if (property.objectReferenceValue == null)
            {
                FindObjectOfTypeAttribute att = (FindObjectOfTypeAttribute)attribute;
                property.objectReferenceValue = UnityEngine.Object.FindObjectOfType(att.Type);
            }

            Default(position, property, label);

        }
    }
#endif

}





