using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using NaughtyAttributes;

[Serializable]
public class EffectingToBasicAbility : Ability
{
    [field: SerializeField] public BasicStatus.BasicStatusTypeList basicStatusTypeTo { set; get; }
}
[Serializable]
public class EffectingToRemainingAbility : Ability
{
    [field: SerializeField] public RemainingStatus.RemainingStatusTypeList RemainingStatusTypeTo { set; get; }
}

[Serializable]
public class Ability
{
    public AbilityData abilityData = new AbilityData();
    [field: SerializeField] public float ModifiedVal { set; get; } = 0f;
    [field: SerializeField] public float ModifiedMultiplyVal { set; get; } = 1f;
    [field: SerializeField] public int ModifiedRepeatTime { set; get; } = 1;
    [field: SerializeField] public float ModifiedDuration { set; get; } = 0f;
    [field: SerializeField] public float CountingUnit { get { return ModifiedDuration / ModifiedRepeatTime; } }
    [field: SerializeField] public float CountingTime { set; get; } = 0f;
    [field: SerializeField] public bool IsTimeOut { get { return CountingTime >= ModifiedDuration; } }
    [field: SerializeField] public TimeCounter.TimeCountData TimeCounterData { set; get; } = null;
}
[Serializable]
public class AbilityData
{
    [field: SerializeField] public SkillData skillData { set; get; }
    public enum ReferenceTypeList { Non, Caster, Target }
    [field: SerializeField] public ReferenceTypeList ReferenceTypeFromCaster { private set; get; } = ReferenceTypeList.Non;
    [field: SerializeField] public ReferenceTypeList ReferenceTypeFromTarget { private set; get; } = ReferenceTypeList.Non;
    [field: SerializeField] public float BasicVal { private set; get; } = 0f;
    [field: SerializeField] public float MultiplyVal { private set; get; } = 1f;
    [field: SerializeField] public int RepeatTime { private set; get; } = 1;
    [field: SerializeField] public float Duration { private set; get; } = 0f;
}