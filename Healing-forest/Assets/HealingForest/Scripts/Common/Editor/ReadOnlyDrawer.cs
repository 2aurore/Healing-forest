using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HF
{
    [CustomEditor(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 읽기 전용 속성의 경우, GUI를 비활성화하여 편집할 수 없도록 합니다.
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }

}
