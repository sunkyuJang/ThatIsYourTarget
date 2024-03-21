using System;
using UnityEngine;
[Serializable]
public class SkillRequirementData
{
    public Model usingObj { set; get; }
    public float lastUsedTime { set; get; }
    public enum RequirementType { Obj, WeaponType, Ablity, SkillLearn, LastSkillUsed, CoolTime, Non }
    public RequirementType requirementType = RequirementType.Non;
    public static string GetRequirementTypeName { get { return "requirementType"; } } // this should be same with requirementType variable Name

    // obj
    [ConditionShowing((int)RequirementType.Obj)][SerializeField] private GameObject RequireOriginalPrefab;

    // Weapon
    [ConditionShowing((int)RequirementType.WeaponType)][SerializeField] private Weapon.WeaponType requireWeaponType = Weapon.WeaponType.Non;
    protected Weapon weapon = null;
    protected Weapon GetWeapon
    {
        get
        {
            if (weapon == null) weapon = usingObj.Weapon;
            return weapon;
        }
    }

    // Ability
    [ConditionShowing((int)RequirementType.Ablity)][SerializeField] private int progress11;

    // Skill learn
    [ConditionShowing((int)RequirementType.SkillLearn)][SerializeField] private int progress12;

    // Last skill used
    [ConditionShowing((int)RequirementType.LastSkillUsed)][SerializeField] private SkillData lastSkillUsed;

    // maxTime
    [ConditionShowing((int)RequirementType.CoolTime)][SerializeField] private float maxTime = 0f;


    public bool IsSatisfy()
    {
        switch (requirementType)
        {
            case RequirementType.WeaponType:
                return GetWeapon != null && weapon.GetWeaponType == requireWeaponType;
            case RequirementType.CoolTime:
                return Time.time - lastUsedTime > maxTime;

            default:
                return false;
        }
    }
}