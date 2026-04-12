using System;
using UnityEditor;
using UnityEngine;

namespace FXEngine.SoundFX.Editor
{
    [CustomPropertyDrawer(typeof(RolloffValue))]
    class RolloffValueDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typeProperty = property.FindPropertyRelative("_type");

            var typePosition = position;
            typePosition.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(typePosition, typeProperty, label);

            if (typeProperty.hasMultipleDifferentValues)
            {
                return;
            }

            switch (typeProperty.enumValueIndex)
            {
                case (int)RolloffValue.Type.Curve:
                    var valuePosition = position;
                    valuePosition.height = EditorGUIUtility.singleLineHeight;
                    valuePosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    using (var _ = new EditorGUI.IndentLevelScope())
                    {
                        valuePosition = EditorGUI.IndentedRect(valuePosition);
                        valuePosition.xMax = position.xMax;

                        var curveProperty = property.FindPropertyRelative("_curve");

                        EditorGUI.PropertyField(valuePosition, curveProperty, GUIContent.none);

                        break;
                    }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var typeProperty = property.FindPropertyRelative("_type");

            if (typeProperty.hasMultipleDifferentValues)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            switch (typeProperty.intValue)
            {
                case (int)RolloffValue.Type.Logarithmic:
                case (int)RolloffValue.Type.Linear:
                    return EditorGUIUtility.singleLineHeight;

                case (int)RolloffValue.Type.Curve:
                    return EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
