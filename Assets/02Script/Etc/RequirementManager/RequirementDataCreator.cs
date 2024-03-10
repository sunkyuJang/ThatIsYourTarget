using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

public class RequirementDataCreator : MonoBehaviour
{
    [HideInInspector] public SkillRequirementData.RequirementType requirement = SkillRequirementData.RequirementType.Non;
    [HideInInspector] public List<SkillRequirementData> requirementDatas = new List<SkillRequirementData>();
}
// RequirementDataCreator 대한 커스텀 에디터를 정의합니다.
[CustomEditor(typeof(RequirementDataCreator))]
public class RequirementDataManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터 UI를 그립니다.
        base.OnInspectorGUI();

        // RequirementDataManager 스크립트를 대상으로 합니다.
        RequirementDataCreator manager = (RequirementDataCreator)target;
        // requirementDatas 리스트를 비활성화된 상태로 표시합니다.
        EditorGUI.BeginDisabledGroup(true);
        SerializedProperty requirementDatasProp = serializedObject.FindProperty("requirementDatas");
        EditorGUILayout.PropertyField(requirementDatasProp, true);
        EditorGUI.EndDisabledGroup();

        // requirement 필드에 대한 드롭다운 메뉴를 만듭니다.
        manager.requirement = (SkillRequirementData.RequirementType)EditorGUILayout.EnumPopup("Requirement Type", manager.requirement);

        // 새 RequirementData를 추가하는 버튼을 만듭니다.
        if (GUILayout.Button("Add New RequirementData"))
        {
            // 새 RequirementData 인스턴스를 생성합니다.
            SkillRequirementData newRequirementData = CreateNewRequirementData(manager.requirement);
            if (newRequirementData != null)
            {
                // 생성된 RequirementData 인스턴스를 requirementDatas 리스트에 추가합니다.
                manager.requirementDatas.Add(newRequirementData);

                // 변경사항을 적용합니다.
                EditorUtility.SetDirty(manager);
                EditorUtility.SetDirty(newRequirementData);
            }
        }

        if (GUILayout.Button("Reset List"))
        {
            // 생성된 RequirementData 인스턴스를 requirementDatas 리스트에 추가합니다.
            manager.requirementDatas.Clear();
        }
    }

    private SkillRequirementData CreateNewRequirementData(SkillRequirementData.RequirementType requirementType)
    {
        // 새 RequirementData ScriptableObject를 생성합니다.
        SkillRequirementData newRequirementData = ScriptableObject.CreateInstance<SkillRequirementData>();
        newRequirementData.requirementType = requirementType;

        string path = "Assets/";
        string requirementName = requirementType.ToString() + ".asset";

        // 적절한 위치에 새 ScriptableObject를 저장합니다.
        string definePath = AssetDatabase.GenerateUniqueAssetPath(path + requirementName);
        AssetDatabase.CreateAsset(newRequirementData, definePath);
        AssetDatabase.SaveAssets();

        return newRequirementData;
    }
}
