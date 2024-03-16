using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Unity.VisualScripting;
[CreateAssetMenu(fileName = "SkillData", menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    [field: SerializeField] public string KeyName { private set; get; } = "";
    [field: SerializeField] public GameObject SkillTargetDetectorObj { private set; get; }
    [field: SerializeField] public GameObject SkillTargetHitterObj { private set; get; }
    public enum SkillEffectStartPointList { Model, Weapon, Non }
    [field: SerializeField] public SkillEffectStartPointList SkillDetectorStartPoint { private set; get; } = SkillEffectStartPointList.Non;
    [field: SerializeField] public SkillEffectStartPointList SkillHitterStartPoint { private set; get; } = SkillEffectStartPointList.Non;
    public enum SkillTypeList { Passive, Active, Non }
    [field: SerializeField] public SkillTypeList SkillType { private set; get; } = SkillTypeList.Non;
    public enum EffectToList { Dmg, MaxHP, Non }
    [field: SerializeField] public EffectToList EffectTo { private set; get; } = EffectToList.Non;
    public enum ElementalTypeList { Normal, Non }
    [field: SerializeField] public ElementalTypeList ElementalType { private set; get; } = ElementalTypeList.Non;
    public enum AdditionalEffectTypeList { Add, Multyply }
    [field: SerializeField] public AdditionalEffectTypeList AdditionalEffectType { private set; get; } = AdditionalEffectTypeList.Add;
    [field: SerializeField] public float AdditionalEffectValue { private set; get; } = 0f;
    [field: SerializeField] public bool CanLoop { private set; get; } = false;

    public RequirementDataManager requirementDataManager = new RequirementDataManager();
}

[CustomEditor(typeof(SkillData))]
public class SkillDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var skillData = target as SkillData;
        skillData = SkillDataDrawer.DrawSkillData(new SerializedObject(skillData), skillData);
    }
}


public static class SkillDataDrawer
{
    public static void DrawData(SerializedObject serializedRequirementData)
    {
        // 속성 순회하며 조건에 따라 그리기
        var property = serializedRequirementData.GetIterator();
        var propertyTarget = serializedRequirementData.FindProperty("requirementDataManager");
        var first = true;
        while (property.NextVisible(first))
        {
            first = false;
            if (property.name != "requirementDataManager")
            {
                EditorGUILayout.PropertyField(property, true);
            }
        }
    }

    public static SkillData DrawSkillData(SerializedObject serializedRequirementData, SkillData skillData)
    {
        // Access the target object (SkillData)
        var property = serializedRequirementData.GetIterator();
        var propertyTarget = serializedRequirementData.FindProperty("requirementDataManager");
        var onlyFristDraw = true;
        while (property.NextVisible(onlyFristDraw))
        {
            onlyFristDraw = false;
            EditorGUILayout.PropertyField(property, true);
        }

        EditorGUILayout.LabelField("", EditorStyles.boldLabel);

        if (skillData.requirementDataManager.RequirementDatas != null)
        {
            if (skillData.requirementDataManager.RequirementDatas.Count > 0)
                EditorGUILayout.LabelField("필수요소 목록", EditorStyles.boldLabel);
            var index = 0;
            skillData.requirementDataManager.RequirementDatas.ForEach(x =>
            {
                if (x != null)
                {
                    // Draw RequirementData
                    EditorGUILayout.LabelField(index++.ToString() + "번쨰 필수요소", EditorStyles.boldLabel);
                    RequirementDataDrawer.DrawRequirementData(x);
                    EditorGUILayout.LabelField("", EditorStyles.boldLabel);
                }
            });
        }

        // 새 RequirementData를 추가하는 버튼을 만듭니다.
        if (GUILayout.Button("새로운 필수 요소 추가"))
        {
            // 새 RequirementData 인스턴스를 생성합니다.
            SkillRequirementData newRequirementData = CreateNewRequirementData(skillData);
            if (newRequirementData != null)
            {
                // 생성된 RequirementData 인스턴스를 requirementDatas 리스트에 추가합니다.
                skillData.requirementDataManager.RequirementDatas.Add(newRequirementData);

                // // 변경사항을 적용합니다.
                EditorUtility.SetDirty(skillData);
                EditorUtility.SetDirty(newRequirementData);
            }
        }

        EditorGUILayout.LabelField("", EditorStyles.boldLabel);
        serializedRequirementData.ApplyModifiedPropertiesWithoutUndo();
        return skillData;
    }

    private static SkillRequirementData CreateNewRequirementData(SkillData skillData)
    {
        // 새 RequirementData ScriptableObject를 생성합니다.
        SkillRequirementData newRequirementData = ScriptableObject.CreateInstance<SkillRequirementData>();

        string path = "Assets/";
        string requirementName = skillData.KeyName + "_Requierment" + ".asset";

        // 적절한 위치에 새 ScriptableObject를 저장합니다.
        string definePath = AssetDatabase.GenerateUniqueAssetPath(path + requirementName);
        AssetDatabase.CreateAsset(newRequirementData, definePath);
        AssetDatabase.SaveAssets();

        return newRequirementData;
    }
}