using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using Unity.VisualScripting;
using Unity.XR.CoreUtils.Editor;
using Unity.Properties;

public class AbsoluteTagSetter : MonoBehaviour
{
    public bool setChildTag = false;
    public void SetChildTag(Transform targetTransform, string mainTag)
    {
        var setter = targetTransform.GetComponent<AbsoluteTagSetter>();
        if (setter == null)
        {
            targetTransform.tag = mainTag;
        }

        if (targetTransform.childCount > 0)
        {
            mainTag = setter != null && setter.setChildTag ? setter.tag : mainTag;

            for (int i = 0; i < targetTransform.childCount; i++)
            {
                var child = targetTransform.GetChild(i);
                var childTagSetter = child.GetComponent<AbsoluteTagSetter>();
                if (childTagSetter == null)
                    SetChildTag(targetTransform.GetChild(i), mainTag);
                else
                {
                    if (childTagSetter.setChildTag)
                    {
                        childTagSetter.SetChildTag(childTagSetter.transform, childTagSetter.tag);
                    }
                    else
                        SetChildTag(targetTransform.GetChild(i), mainTag);
                }
            }
        }
    }
}

[CustomEditor(typeof(AbsoluteTagSetter))]
public class AbsoluteTagSetterEditor : Editor
{
    int selectedIndex = 0;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var _target = (AbsoluteTagSetter)target;

        // 현재 설정된 태그 목록 표시
        var tags = InternalEditorUtility.tags;
        // var settedIndex = -1;
        // for (int i = 0; i < tags.Length; i++)
        // {
        //     if (_target.CompareTag(tags[i]))
        //     {
        //         settedIndex = i;
        //         break;
        //     }
        // }

        // 태그를 선택하는 드롭다운 메뉴 생성
        selectedIndex = EditorGUILayout.Popup("Select Tag", selectedIndex, tags);
        if (GUILayout.Button("Set Tag"))
        {
            if (selectedIndex != -1) // 태그가 선택되었다면
            {
                _target.tag = tags[selectedIndex]; // 선택한 태그를 AbsoluteTagSetter의 tag에 할당
                _target.SetChildTag(_target.transform, _target.tag);
            }
        }
    }


}