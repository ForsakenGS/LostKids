﻿using UnityEngine;
using System.Collections;
using UnityEditor;

public class Flags : PropertyAttribute {

     public Flags() { }

    [CustomPropertyDrawer(typeof(Flags))]
    public class EnumFlagsDrawer : PropertyDrawer   {

        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label) {
            _property.intValue = EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
        }
    }
}
