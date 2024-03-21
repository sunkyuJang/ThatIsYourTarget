using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class RemainingAbility
{
    [field: SerializeField] public List<RemainingStatus> RemainingStatusList { set; get; } = new List<RemainingStatus>();
    public RemainingAbility()
    {

    }
}
