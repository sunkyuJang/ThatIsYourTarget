using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using JExtentioner;
[CreateAssetMenu(fileName = "SkillRequirementData", menuName = "Requirements/SkillRequirementData")]
public class SkillRequirementData : ScriptableObject
{
    public Model usingObj { set; get; }
    public float lastUsedTime { set; get; }
    public enum RequirementType { Obj, WeaponType, Ablity, SkillLearn, LastSkillUsed, CoolTime, Non }
    [HideInInspector] public RequirementType requirementType = RequirementType.Non;
    public static string GetRequirementTypeName { get { return "requirementType"; } } // this should be same with requirementType variable Name

    // obj
    [ConditionShowing((int)RequirementType.Obj)][SerializeField] private GameObject RequireOriginalPrefab;

    // Weapon
    [ConditionShowing((int)RequirementType.WeaponType)][SerializeField] private Weapon.WeaponType requireWeaponType = Weapon.WeaponType.Non;
    protected Weapon weapon = null;
    protected Weapon GetWeapon
    {
        get
        {
            if (weapon == null) weapon = usingObj.Weapon;
            return weapon;
        }
    }

    // Ability
    [ConditionShowing((int)RequirementType.Ablity)][SerializeField] private int progress11;

    // Skill learn
    [ConditionShowing((int)RequirementType.SkillLearn)][SerializeField] private int progress12;

    // Last skill used
    [ConditionShowing((int)RequirementType.LastSkillUsed)][SerializeField] private SkillData lastSkillUsed;

    // maxTime
    [ConditionShowing((int)RequirementType.CoolTime)][SerializeField] private float maxTime = 0f;


    public bool IsSatisfy()
    {
        switch (requirementType)
        {
            case RequirementType.WeaponType:
                return GetWeapon != null && weapon.GetWeaponType == requireWeaponType;
            case RequirementType.CoolTime:
                return Time.time - lastUsedTime > maxTime;

            default:
                return false;
        }
    }
}

[CustomEditor(typeof(SkillRequirementData))]
public class RequirementDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RequirementDataDrawer.DrawRequirementData(target as SkillRequirementData);
    }
}
public static class RequirementDataDrawer
{
    public static void DrawRequirementData(SkillRequirementData skillData)
    {
        SerializedObject serializedRequirementData = new SerializedObject(skillData);
        var requirementVariableName = nameof(skillData.requirementType);
        // 'requirement' 속성 그리기
        EditorGUILayout.PropertyField(serializedRequirementData.FindProperty(requirementVariableName));

        // 속성 순회하며 조건에 따라 그리기
        var property = serializedRequirementData.GetIterator();
        bool isInConditionalRegion = false;
        int currentRequirement = serializedRequirementData.FindProperty(requirementVariableName).enumValueIndex;

        while (property.NextVisible(true))
        {
            FieldInfo fieldInfo = typeof(SkillRequirementData).GetField(property.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                var startAttrs = fieldInfo.GetCustomAttributes(typeof(ConditionShowing), false);
                if (startAttrs.Length > 0)
                {
                    var startAttr = (ConditionShowing)startAttrs[0];
                    isInConditionalRegion = true;

                    if (startAttr.conditionIndex == currentRequirement)
                    {
                        EditorGUILayout.PropertyField(property, true);
                    }
                    continue;
                }
            }

            if (!isInConditionalRegion || (isInConditionalRegion && currentRequirement == serializedRequirementData.FindProperty(requirementVariableName).enumValueIndex))
            {
                EditorGUILayout.PropertyField(property, true);
            }
        }

        serializedRequirementData.ApplyModifiedProperties();
    }
}