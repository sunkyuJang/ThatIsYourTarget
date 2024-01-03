using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class SkillLoader_Person : SkillLoader
{
    public SkillLoader_Person(AnimatorController animatorController, Model usingModel) : base(animatorController, usingModel)
    {
    }

    public override SkillData SeletSkillForEachModel(List<SkillData> avaliavleSkills)
    {
        return avaliavleSkills[0];
    }
}
