using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Unity.VisualScripting;
using System.Linq.Expressions;
using Unity.XR.CoreUtils;
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
    [field: SerializeField] public bool CanLoop { private set; get; } = false;
    public List<EffectingToBasicAbility> effectingToBasicAbilities = new List<EffectingToBasicAbility>();
    public List<EffectingToRemainingAbility> effectingToRemainingAbility = new List<EffectingToRemainingAbility>();
    public List<SkillRequirementData> requirementDatas = new List<SkillRequirementData>();

    public bool showEffecingToBasicAbilityDatas = false;
    public bool showEffecingToRemainingAbilityDatas = false;
    public bool propertyShowRequirementDatas = false;
    void OnEnable()
    {
        effectingToBasicAbilities.ForEach(x => x.abilityData.skillData = this);
        effectingToRemainingAbility.ForEach(x => x.abilityData.skillData = this);
    }

    public bool IsSatisfy(Model usingModel, float lastUsedTime)
    {
        var result = true;
        foreach (var x in requirementDatas)
        {
            x.usingObj = usingModel;
            x.lastUsedTime = lastUsedTime;
            result = x.IsSatisfy();

            if (!result)
                break;
        }

        return result;
    }
}

[CustomEditor(typeof(SkillData))]
public class SkillDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        var skillData = target as SkillData;
        DrawSkillData(new SerializedObject(skillData), skillData);
    }
    public void DrawSkillData(SerializedObject serializedRequirementData, SkillData skillData)
    {
        // Access the target object (SkillData)
        var property = serializedRequirementData.GetIterator();
        var propertyEffectingToBasicAbilityDataTarget = serializedRequirementData.FindProperty(nameof(skillData.effectingToBasicAbilities));
        var propertyShowEffecingToBasicAbilityDatas = serializedRequirementData.FindProperty(nameof(skillData.showEffecingToBasicAbilityDatas));

        var propertyEffectingToRemainingAbilityDataTarget = serializedRequirementData.FindProperty(nameof(skillData.effectingToRemainingAbility));
        var propertyShowEffecingToRemainingAbilityDatas = serializedRequirementData.FindProperty(nameof(skillData.showEffecingToRemainingAbilityDatas));

        var propertyRequirementDataTarget = serializedRequirementData.FindProperty(nameof(skillData.requirementDatas));
        var propertyShowRequirementDataTarget = serializedRequirementData.FindProperty(nameof(skillData.propertyShowRequirementDatas));


        var onlyFristDraw = true;
        while (property.NextVisible(onlyFristDraw))
        {
            onlyFristDraw = false;
            if (property.name == propertyEffectingToRemainingAbilityDataTarget.name ||
                    property.name == propertyShowEffecingToRemainingAbilityDatas.name ||
                    property.name == propertyEffectingToBasicAbilityDataTarget.name ||
                    property.name == propertyShowEffecingToBasicAbilityDatas.name ||
                    property.name == propertyRequirementDataTarget.name ||
                    property.name == propertyShowRequirementDataTarget.name
                    ) { }
            else
            {
                EditorGUILayout.PropertyField(property, true);
            }
        }

        DrawEffectAbilityList(ref skillData.effectingToBasicAbilities, propertyEffectingToBasicAbilityDataTarget, propertyShowEffecingToBasicAbilityDatas);
        DrawEffectAbilityList(ref skillData.effectingToRemainingAbility, propertyEffectingToRemainingAbilityDataTarget, propertyShowEffecingToRemainingAbilityDatas);

        // Essential List
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Show " + propertyShowRequirementDataTarget.displayName, EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(propertyShowRequirementDataTarget, new GUIContent(""), true);
        EditorGUILayout.EndHorizontal();
        if (propertyShowRequirementDataTarget.boolValue)
        {
            // 새 RequirementData를 추가하는 버튼을 만듭니다.
            if (GUILayout.Button("새로운 필수 요소 추가"))
            {
                // 새 RequirementData 인스턴스를 생성합니다.
                SkillRequirementData newRequirementData = new SkillRequirementData();
                if (newRequirementData != null)
                {
                    // 생성된 RequirementData 인스턴스를 requirementDatas 리스트에 추가합니다.
                    skillData.requirementDatas.Add(newRequirementData);

                    // // 변경사항을 적용합니다.
                    EditorUtility.SetDirty(skillData);
                }
            }

            if (skillData.requirementDatas.Count > 0)
            {
                List<int> indexes = new List<int>();
                if (skillData.requirementDatas.Count > 0)
                    EditorGUILayout.LabelField("필수요소 목록", EditorStyles.boldLabel);
                var index = 0;
                skillData.requirementDatas.ForEach(x =>
                {
                    if (x != null)
                    {
                        // Draw RequirementData
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(index.ToString() + "번쨰 필수요소", EditorStyles.boldLabel);
                        if (GUILayout.Button("-", GUILayout.Width(20)))
                        {
                            indexes.Add(index);
                        }
                        EditorGUILayout.EndHorizontal();
                        var listProperty = serializedRequirementData.FindProperty(nameof(skillData.requirementDatas));
                        for (int i = 0; i < listProperty.arraySize; i++)
                        {
                            SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);
                            var depth = elementProperty.depth;
                            while (elementProperty.NextVisible(true))
                            {
                                FieldInfo fieldInfo = typeof(SkillRequirementData).GetField(elementProperty.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                                if (fieldInfo != null)
                                {
                                    if (fieldInfo.Name == nameof(x.requirementType))
                                    {
                                        EditorGUILayout.PropertyField(elementProperty, true);
                                    }
                                    var startAttrs = fieldInfo.GetCustomAttributes(typeof(ConditionShowing), false);
                                    if (startAttrs.Length > 0)
                                    {
                                        var startAttr = (ConditionShowing)startAttrs[0];
                                        if (startAttr.conditionIndex == (int)x.requirementType)
                                        {
                                            EditorGUILayout.PropertyField(elementProperty, true);
                                        }
                                    }
                                }
                                //EditorGUILayout.PropertyField(elementProperty, true);
                            }
                        }
                        EditorGUILayout.LabelField("", EditorStyles.boldLabel);
                    }
                });

                if (indexes.Count > 0)
                {
                    indexes.ForEach(x => skillData.requirementDatas.RemoveAt(x));
                }
            }
            else
            {
                EditorGUILayout.LabelField("list is empty");
            }
        }

        serializedRequirementData.ApplyModifiedPropertiesWithoutUndo();
    }

    private void DrawEffectAbilityList<T>(ref List<T> abilities, SerializedProperty listProperty, SerializedProperty showListProperty) where T : Ability
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Show " + listProperty.displayName, EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(showListProperty, new GUIContent(""), true);
        EditorGUILayout.EndHorizontal();

        List<int> indexes = new List<int>();
        if (showListProperty.boolValue)
        {
            if (GUILayout.Button("Add(+)"))
            {
                if (abilities is List<EffectingToBasicAbility>)
                {
                    var ability = new EffectingToBasicAbility();
                    ability.abilityData.skillData = target as SkillData;
                    (abilities as List<EffectingToBasicAbility>).Add(ability);
                }
                else
                {
                    var ability = new EffectingToRemainingAbility();
                    ability.abilityData.skillData = target as SkillData;
                    (abilities as List<EffectingToRemainingAbility>).Add(ability);
                }
            }

            if (listProperty.arraySize == 0)
            {
                EditorGUILayout.LabelField("list is empty");
            }
            else
            {
                for (int i = 0; i < listProperty.arraySize; i++)
                {
                    var ability = abilities[i] as Ability;
                    SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);

                    EditorGUILayout.BeginHorizontal();
                    while (elementProperty.NextVisible(true))
                    {
                        if (elementProperty.name == nameof(ability.abilityData))
                        {
                            EditorGUILayout.PropertyField(elementProperty, true);
                            break;
                        }
                    }
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        indexes.Add(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.LabelField("");
        }

        if (indexes.Count > 0)
        {
            indexes.ForEach(x => listProperty.DeleteArrayElementAtIndex(x));
        }
    }
}