using UnityEngine;
using UnityEditor;
public class ReadOnlyAttribute : PropertyAttribute
{

}
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false; // 이 부분이 인스펙터에서 수정 불가능하게 만듭니다.
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}