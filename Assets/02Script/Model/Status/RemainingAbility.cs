using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class RemainingAbility
{
    [field: SerializeField] public List<RemainingStatus> RemainingStatusList { set; get; } = new List<RemainingStatus>();
    public void SetRemainingAbility(EffectingToRemainingAbility remainingAbility)
    {
        foreach (var status in RemainingStatusList)
        {
            if (remainingAbility.RemainingStatusTypeTo.Equals(status.RemainingStatusType))
            {
                status.AddingEffectingToAbility(remainingAbility);
                break;
            }
        }
    }
}
