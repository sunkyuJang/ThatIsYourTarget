using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Unity.VisualScripting;
[CreateAssetMenu(fileName = "SkillData", menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    [SerializeField] public string keyName;
    [SerializeField] public string displayedName;
    public enum SkillType { Passive, Active, Non }
    [SerializeField] private SkillType skillType = SkillType.Non;
    public enum EffectTo { Dmg, MaxHP, Non }
    [SerializeField] public EffectTo effectTo = EffectTo.Non;
    public enum ElementalType { Normal, Non }
    [SerializeField] private ElementalType elementalType = ElementalType.Non;
    public enum AdditionalEffectType { Add, Multyply }
    [SerializeField] public AdditionalEffectType additionalEffectType = AdditionalEffectType.Add;
    [SerializeField] private float value = 0f;
    public bool canLoop = false;
    public int poolerStayCount = 0;

    public RequirementDataManager requirementDataManager = new RequirementDataManager();
    public GameObject SkillDetectorObj;
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
                    SerializedObject serializedRequirementData = new SerializedObject(x);
                    RequirementDataDrawer.DrawRequirementData(serializedRequirementData);
                    serializedRequirementData.ApplyModifiedProperties();
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
        string requirementName = skillData.keyName + "_Requierment" + ".asset";

        // 적절한 위치에 새 ScriptableObject를 저장합니다.
        string definePath = AssetDatabase.GenerateUniqueAssetPath(path + requirementName);
        AssetDatabase.CreateAsset(newRequirementData, definePath);
        AssetDatabase.SaveAssets();

        return newRequirementData;
    }
}