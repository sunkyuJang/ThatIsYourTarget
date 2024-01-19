using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Rendering;
using System.Linq;

public class SkillManager : MonoBehaviour
{
    public AnimatorController animatorController;
    [SerializeField] private SerializedDictionary<string, SkillData> skills = new SerializedDictionary<string, SkillData>();
    [SerializeField] private SerializedDictionary<SkillData, ObjPooler> skillDetectors = new SerializedDictionary<SkillData, ObjPooler>();

    public SerializedDictionary<string, SkillData> AllSkillData { get => skills; }
    public void Initialize(AnimatorController controller)
    {
        skills.Clear();
        animatorController = controller;
        var list = controller.GetBehaviours<SkillConnector_Animator>();
        foreach (var item in list)
        {
            if (item.skillData != null)
                skills.Add(item.skillData.keyName, item.skillData);
        }
    }
    public void CheckAllSkillName(List<string> skillKeyNames)
    {
        foreach (var keyName in skills.Keys)
        {
            if (!skillKeyNames.Contains(keyName))
            {
                Debug.Log("skillkeyName must same with animationStateName_ target SkillName : " + keyName);
            }
        }
    }
    private void Start()
    {
        foreach (var skillData in skillDetectors.Keys)
        {
            var skillPooler = ObjPoolerManager.Instance.GetPooler(skillData.SkillDetectorObj);
            for (int i = 0; i < skillData.poolerStayCount; i++)
            {
                skillPooler.MakeNewOne();
            }
            skillDetectors.Add(skillData, skillPooler);
        }
    }

    public SkillTargetDetector GetSkillTargetDetector(SkillData skillData)
    {
        if (skillDetectors.ContainsKey(skillData))
            return skillDetectors[skillData].GetNewOne<SkillTargetDetector>();

        return null;
    }

    public class SkillToken
    {
        public SkillData SkillData;
        public Model usingModel;
        public List<Model> targeModel;
        public float timeStemp;
    }
}
