using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FXEngine.SoundFX.Editor
{
    [CustomPropertyDrawer(typeof(FloatValue))]
    class FloatValueDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typeProperty = property.FindPropertyRelative("_type");

            var typePosition = position;
            typePosition.height = EditorGUIUtility.singleLineHeight;

            var valuePosition = position;
            valuePosition.height = EditorGUIUtility.singleLineHeight;
            valuePosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.PropertyField(typePosition, typeProperty, label);

            if (typeProperty.hasMultipleDifferentValues)
            {
                return;
            }

            switch (typeProperty.enumValueIndex)
            {
                case (int)FloatValue.Type.Constant:
                    valuePosition.x += EditorGUIUtility.labelWidth;
                    valuePosition.width -= EditorGUIUtility.labelWidth;

                    var constantValueProperty = property.FindPropertyRelative("_constantValue");

                    var attr = fieldInfo.GetCustomAttribute<FloatValueRangeAttribute>();
                    if (attr != null)
                    {
                        EditorGUI.Slider(valuePosition, constantValueProperty, attr.Min, attr.Max, GUIContent.none);
                    }
                    else
                    {
                        EditorGUI.PropertyField(valuePosition, constantValueProperty, GUIContent.none);
                    }

                    break;

                case (int)FloatValue.Type.Curve:
                    using (var _ = new EditorGUI.IndentLevelScope())
                    {
                        valuePosition = EditorGUI.IndentedRect(valuePosition);
                        valuePosition.xMax = position.xMax;

                        var curveProperty = property.FindPropertyRelative("_curve");

                        EditorGUI.PropertyField(valuePosition, curveProperty, GUIContent.none);

                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var typeProperty = property.FindPropertyRelative("_type");

            if (typeProperty.hasMultipleDifferentValues)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
