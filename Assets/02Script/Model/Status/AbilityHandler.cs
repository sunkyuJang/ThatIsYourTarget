using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using System.Linq;
using JExtentioner;

[Serializable]
public class AbilityHandler : MonoBehaviour
{
    [field: SerializeField] public BasicAbility BasicAbility { set; get; } = new BasicAbility();
    [field: SerializeField] public RemainingAbility RemainingAbility { set; get; } = new RemainingAbility();
    AbilityListener[] AbilityListner { set; get; }

    public void SetBasicAbility(EffectingToBasicAbility effectingToBasicAbility)
    {

    }

    public void SetRemainingAbility(EffectingToRemainingAbility effecingToRemainingAbility)
    {
        switch (effecingToRemainingAbility.abilityData.ReferenceTypeFromTarget)
        {
            case AbilityData.ReferenceTypeList.Non:
                RemainingAbility.SetRemainingAbility(effecingToRemainingAbility);
                break;
            case AbilityData.ReferenceTypeList.Target:
                CalcRemainigAbility(effecingToRemainingAbility);
                break;
        }
    }

    public void SetAbilityConnector(Transform actorTransform)
    {
        var rigidbodies = actorTransform.GetComponentsInChildren<Rigidbody>();
        AbilityListner = new AbilityListener[rigidbodies.Length];
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            var item = rigidbodies[i];
            AbilityListner[i] = item.gameObject.AddComponent<AbilityListener>();
            AbilityListner[i].abilityHandler = this;
        }
    }

    void CalcRemainigAbility(EffectingToRemainingAbility effecingToRemainingAbility)
    {
        RemainingAbility.SetRemainingAbility(effecingToRemainingAbility);
    }
}

[CustomEditor(typeof(AbilityHandler))]
public class AbilityHandlerEditor : Editor
{
    private BasicStatus.BasicStatusTypeList selectedBasicStatusType = BasicStatus.BasicStatusTypeList.HP; // 초기 선택 값
    private RemainingStatus.RemainingStatusTypeList selectedRemainingStatusType = RemainingStatus.RemainingStatusTypeList.HP; // 초기 선택 값
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("BasicAbility");
        selectedBasicStatusType = (BasicStatus.BasicStatusTypeList)EditorGUILayout.EnumPopup("Basic Status Type", selectedBasicStatusType);
        ControlBasicStatus();

        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("RemaningAbility");
        selectedRemainingStatusType = (RemainingStatus.RemainingStatusTypeList)EditorGUILayout.EnumPopup("Basic Status Type", selectedRemainingStatusType);
        ControlRemainingStatus();
    }

    void ControlBasicStatus()
    {
        AbilityHandler abilityHandler = (AbilityHandler)target;
        BasicAbility basicAbility = abilityHandler.BasicAbility;
        if (GUILayout.Button("Adding BasicAbility"))
        {
            // 중복 추가 방지
            var basicStatus = basicAbility.BasicStatusList.Find(x => x.BasicStatusType == selectedBasicStatusType);
            if (basicStatus == null)
            {
                basicAbility.BasicStatusList.Add(new BasicStatus(selectedBasicStatusType, 0f));
                EditorUtility.SetDirty(target); // 변경된 값 저장
            }
            else
            {
                Debug.LogWarning("해당 타입의 BasicStatus가 이미 존재합니다.");
            }
        }

        if (GUILayout.Button("Remove BasicAbility"))
        {
            var basicStatus = basicAbility.BasicStatusList.Find(x => x.BasicStatusType == selectedBasicStatusType);
            if (basicStatus != null)
            {
                basicAbility.BasicStatusList.Remove(basicStatus);
                EditorUtility.SetDirty(target); // 변경된 값 저장
            }
            else
            {
                Debug.LogWarning("해당 타입의 BasicStatus가 존재하지 않습니다.");
            }
        }
    }
    void ControlRemainingStatus()
    {
        AbilityHandler abilityHandler = (AbilityHandler)target;
        BasicAbility basicAbility = abilityHandler.BasicAbility;
        RemainingAbility remainingAbility = abilityHandler.RemainingAbility;

        if (GUILayout.Button("Load RemainingAbility from Basic Ability"))
        {
            var count = 0;
            foreach (var basicStatus in basicAbility.BasicStatusList)
            {
                for (int i = 0; i < EnumExtentioner.GetEnumSize<RemainingStatus.RemainingStatusTypeList>(); i++)
                {
                    var targetType = EnumExtentioner.GetEnumVal<RemainingStatus.RemainingStatusTypeList>(i);
                    if (targetType != null)
                    {
                        var targetTypeCopy = targetType ?? RemainingStatus.RemainingStatusTypeList.HP;
                        if (basicStatus.BasicStatusType.ToString() == targetType.ToString())
                        {
                            RemainingStatus remainingStatus = remainingAbility.RemainingStatusList.Find(x => x.RemainingStatusType == targetType);
                            if (remainingStatus != null)
                            {
                                remainingStatus.NowVal = basicStatus.TotalVal;
                                remainingStatus.MaxVal = basicStatus.TotalVal;
                            }
                            else
                            {
                                remainingStatus = new RemainingStatus(targetTypeCopy, basicStatus.TotalVal, basicStatus.TotalVal);
                                abilityHandler.RemainingAbility.RemainingStatusList.Add(remainingStatus);
                            }
                            count++;
                            break;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Something wrong when get type from ." + nameof(RemainingStatus.RemainingStatusTypeList));
                    }
                }
            }

            if (count == 0)
            {
                Debug.LogWarning("couldnt find anythink. plz, check the pair of type List between basicAbility and remainingAbility.");
            }
        }

        if (GUILayout.Button("Adding RemainingAbility"))
        {
            // 중복 추가 방지
            RemainingStatus remainingStatus = remainingAbility.RemainingStatusList.Find(x => x.RemainingStatusType == selectedRemainingStatusType);
            if (remainingStatus == null)
            {
                remainingAbility.RemainingStatusList.Add(new RemainingStatus(selectedRemainingStatusType, 0f, 0f));
                EditorUtility.SetDirty(target); // 변경된 값 저장
            }
            else
            {
                Debug.LogWarning("해당 타입의 BasicStatus가 이미 존재합니다.");
            }
        }

        if (GUILayout.Button("Remove RemainingAbility"))
        {
            RemainingStatus remainingStatus = remainingAbility.RemainingStatusList.Find(x => x.RemainingStatusType == selectedRemainingStatusType);
            if (remainingStatus != null)
            {
                remainingAbility.RemainingStatusList.Remove(remainingStatus);
                EditorUtility.SetDirty(target); // 변경된 값 저장
            }
            else
            {
                Debug.LogWarning("해당 타입의 BasicStatus가 존재하지 않습니다.");
            }
        }
    }
}