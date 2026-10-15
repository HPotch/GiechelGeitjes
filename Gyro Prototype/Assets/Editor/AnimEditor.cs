using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Anim))]
public class AnimDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float propertyHeight = EditorGUI.GetPropertyHeight(property, true);
        
        if (property.isExpanded)
        {
            float buttonHeight = EditorGUIUtility.singleLineHeight + 4f;
            return propertyHeight + buttonHeight;
        }
        
        return propertyHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float propertyHeight = EditorGUI.GetPropertyHeight(property, true);
        Rect propertyRect = new Rect(position.x, position.y, position.width, propertyHeight);
        
        EditorGUI.PropertyField(propertyRect, property, label, true);

        if (property.isExpanded)
        {
            Rect buttonRect = new Rect(position.x, position.y + propertyHeight + 2f, position.width, EditorGUIUtility.singleLineHeight);
            
            if (GUI.Button(buttonRect, "Test"))
            {
                Anim targetAnim = (Anim)property.boxedValue;
    
                if (targetAnim != null)
                {
                    CrashManager.Instance.StartAnimation(targetAnim);
                }
            }
        }

        EditorGUI.EndProperty();
    }
}