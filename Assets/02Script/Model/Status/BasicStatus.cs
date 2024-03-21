using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class BasicStatus
{
    public enum BasicStatusTypeList { HP, MP, ATK, DEF }
    [field: SerializeField] public BasicStatusTypeList BasicStatusType { private set; get; } = BasicStatusTypeList.HP;
    [field: SerializeField] public float originalVal { set; get; } = 0f;
    [field: SerializeField] public float additionalVal { set; get; } = 0f;
    [field: SerializeField] public float MultiplyVal { set; get; } = 0f;
    [field: SerializeField] public float TotalVal { get { return (originalVal + additionalVal) + (originalVal + additionalVal) * MultiplyVal; } }
    [field: SerializeField] List<EffectingToBasicAbility> EffectingList { set; get; } = new List<EffectingToBasicAbility>();

    public BasicStatus(BasicStatusTypeList basicStatusType, float originalVal)
    {
        this.BasicStatusType = basicStatusType;
        this.originalVal = originalVal;
    }

    public void AddingEffectingToBasicAbility(EffectingToBasicAbility ability)
    {

    }
}
