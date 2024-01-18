using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxAttribute))]
public class MinMaxDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (property.propertyType == SerializedPropertyType.Vector2) {
            MinMaxAttribute range = attribute as MinMaxAttribute;

            EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label);
            EditorGUI.BeginProperty(position, label, property);

            float minValue = property.vector2Value.x;
            float maxValue = property.vector2Value.y;

            position.y += EditorGUIUtility.singleLineHeight;
            Rect minValueRect = new Rect(position.x, position.y, 40f, EditorGUIUtility.singleLineHeight);
            minValue = EditorGUI.FloatField(minValueRect, minValue);

            Rect sliderRect = new Rect(position.x + 45f, position.y, position.width - 90f, EditorGUIUtility.singleLineHeight);
            EditorGUI.MinMaxSlider(sliderRect, ref minValue, ref maxValue, range.min, range.max);

            Rect maxValueRect = new Rect(position.xMax - 40f, position.y, 40f, EditorGUIUtility.singleLineHeight);
            maxValue = EditorGUI.FloatField(maxValueRect, maxValue);

            property.vector2Value = new Vector2(minValue, maxValue);

            EditorGUI.EndProperty();
        }else if (property.propertyType == SerializedPropertyType.Vector2Int) {
            MinMaxAttribute range = attribute as MinMaxAttribute;

            EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label);
            EditorGUI.BeginProperty(position, label, property);

            int minValue = property.vector2IntValue.x;
            int maxValue = property.vector2IntValue.y;

            position.y += EditorGUIUtility.singleLineHeight;
            Rect minValueRect = new Rect(position.x, position.y, 40f, EditorGUIUtility.singleLineHeight);
            minValue = EditorGUI.IntField(minValueRect, minValue);

            float f_minValue = minValue;
            float f_maxValue = maxValue;

            Rect sliderRect = new Rect(position.x + 45f, position.y, position.width - 90f, EditorGUIUtility.singleLineHeight);
            EditorGUI.MinMaxSlider(sliderRect, ref f_minValue, ref f_maxValue, range.min, range.max);

            minValue = Mathf.RoundToInt(f_minValue);
            maxValue = Mathf.RoundToInt(f_maxValue);

            Rect maxValueRect = new Rect(position.xMax - 40f, position.y, 40f, EditorGUIUtility.singleLineHeight);
            maxValue = EditorGUI.IntField(maxValueRect, maxValue);

            property.vector2IntValue = new Vector2Int(minValue, maxValue);

            EditorGUI.EndProperty();
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return base.GetPropertyHeight(property, label) * 2;
    }
}