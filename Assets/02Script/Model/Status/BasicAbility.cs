using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class BasicAbility
{
    [field: SerializeField] public List<BasicStatus> BasicStatusList { set; get; } = new List<BasicStatus>();
    public BasicAbility()
    {

    }
}