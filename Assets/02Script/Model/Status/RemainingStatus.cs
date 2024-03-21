using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class RemainingStatus
{
    public enum RemainingStatusTypeList { HP, MP, Stamina, Shild }
    [field: SerializeField] public RemainingStatusTypeList RemainingStatusType { set; get; } = RemainingStatusTypeList.HP;
    [field: SerializeField] public float MaxVal { set; get; } = 0f;
    [field: SerializeField] public float NowVal { set; get; } = 0f;

    [field: SerializeField] List<Ability> EffectingList { set; get; } = new List<Ability>();
    public RemainingStatus(RemainingStatusTypeList statusType, float maxVal, float nowVal)
    {
        this.RemainingStatusType = statusType;
        this.MaxVal = maxVal;
        this.NowVal = nowVal;
    }

    public void AddingEffectingToAbility(Ability ability)
    {
        // if duplicated skill cast, restart TimeCount
        var duplicatedEffectingAbility = EffectingList.Find(x => x.abilityData == ability.abilityData);
        if (duplicatedEffectingAbility != null)
        {
            duplicatedEffectingAbility.CountingTime = 0;
            TimeCounter.Instance.StopTimeCounting(duplicatedEffectingAbility.TimeCounterData);
        }

        ApplyAbility(ability);
    }

    void ApplyAbility(Ability ability)
    {
        NowVal += ability.ModifiedVal * ability.ModifiedMultiplyVal;

        if (ability.ModifiedDuration > 0f)
        {
            ability.CountingTime += ability.CountingUnit;

            var timeData = TimeCounter.Instance.SetTimeCounting(ability.CountingUnit, () => ApplyAbility(ability));
            ability.TimeCounterData = timeData;

            var effectingAbility = EffectingList.Find(x => x.abilityData == ability.abilityData);
            if (effectingAbility != null)
            {
                if (ability.CountingTime < ability.ModifiedDuration)
                {
                    effectingAbility.TimeCounterData = timeData;
                }
                else
                    EffectingList.Remove(effectingAbility);
            }
            else
            {
                EffectingList.Add(ability);
            }
        }
    }
}

