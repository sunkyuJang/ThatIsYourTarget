using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using System;

public abstract class SkillLoader
{
    protected AnimatorController animatorController;
    protected AttackingAnimationStateManager attackingAnimationStateManager;
    protected SkillManager skillManager { get { return attackingAnimationStateManager.skillManager; } }
    protected Model usingModel;
    protected Dictionary<SkillData, SkillToken> usingSkill = new Dictionary<SkillData, SkillToken>();
    public SkillLoader(AnimatorController animatorController, Model usingModel)
    {
        this.animatorController = animatorController;
        this.usingModel = usingModel;

        attackingAnimationStateManager = AnimatorStateManager.Instance.GetAttackingStateManager(animatorController);
    }

    public SkillData SelectSkill()
    {
        var avaliableSkills = GetAvailableSkillData();
        return SeletSkillForEachModel(avaliableSkills);
    }
    public List<SkillData> GetAvailableSkillData()
    {
        List<SkillData> avaliableSkills = new List<SkillData>();
        var AllSkillData = skillManager.AllSkillData;

        foreach (var skill in AllSkillData.Values)
        {
            if (usingSkill.ContainsKey(skill))
            {
                if (skill.IsSatisfy(usingModel, usingSkill[skill].timeStemp))
                {
                    avaliableSkills.Add(skill);
                    usingSkill.Remove(skill);
                }
            }
            else
            {
                if (skill.IsSatisfy(usingModel, 0f))
                    avaliableSkills.Add(skill);
            }
        }

        return avaliableSkills;
    }

    public abstract SkillData SeletSkillForEachModel(List<SkillData> avaliavleSkills);

    public SkillToken UseSkill()
    {
        var skillData = SelectSkill();
        var skillToken = new SkillToken
        {
            SkillData = skillData,
            usingModel = usingModel,
            timeStemp = Time.time,
            GetTargetDetector = skillManager.GetSkillTargetDetector,
            RestoreTargetDetector = skillManager.RestoreSkillTargetDetector,
            GetTargetHitter = skillManager.GetSKillTargetHitter,
            RestoreTargetHitter = skillManager.RestoreSkillTargetHitter,
        };

        if (usingSkill.ContainsKey(skillData))
        {
            usingSkill[skillData] = skillToken;
        }
        else
        {
            usingSkill.Add(skillData, skillToken);
        }

        return skillToken;
    }
    public class SkillToken
    {
        public SkillData SkillData;
        public Model usingModel;
        public List<Model> targeModel;
        public float timeStemp;
        public int curLoopCount;
        public int maxLoopCount;
        public Func<SkillData, SkillTargetDetector> GetTargetDetector;
        public Action<SkillData, SkillTargetDetector> RestoreTargetDetector;
        public Func<SkillData, SkillTargetHitter> GetTargetHitter;
        public Action<SkillData, SkillTargetHitter> RestoreTargetHitter;
    }
}
