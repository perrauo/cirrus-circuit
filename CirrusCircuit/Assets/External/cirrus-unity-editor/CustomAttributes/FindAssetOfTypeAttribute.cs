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
    public class FindAssetOfTypeAttribute : PropertyAttribute
    {
        public Type Type;

        public FindAssetOfTypeAttribute(Type type)
        {
            Type = type; 
        }

    }


#if UNITY_EDITOR

    //Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
    //Modified by: Sebastian Lague


    [CustomPropertyDrawer(typeof(FindAssetOfTypeAttribute))]
    public class FindAssetOfTypeDrawer : PropertyDrawer
    {
        public void Default(Rect position, SerializedProperty property, GUIContent label)
        {
            RefEditorGUI.DefaultPropertyField(position, property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        { 
            if (property.objectReferenceValue == null)
            {
                FindAssetOfTypeAttribute att = (FindAssetOfTypeAttribute)attribute;
                property.objectReferenceValue = AssetDatabaseUtils.FindObjectOfType(att.Type);
                Debug.Log(property.objectReferenceValue);
            }

            Default(position, property, label);
        }
    }

#endif

}





