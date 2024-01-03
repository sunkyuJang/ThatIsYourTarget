using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using Unity.VisualScripting;

[Serializable]
public class RequirementDataManager
{
    [SerializeField] private List<SkillRequirementData> requirementDatas = new List<SkillRequirementData>();
    public List<SkillRequirementData> RequirementDatas { get => requirementDatas; }

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
