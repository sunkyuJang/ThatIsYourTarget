using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Rendering;
using System.Linq;
using Unity.VisualScripting;
using Unity.Mathematics;

public class SkillManager : MonoBehaviour
{
    public AnimatorController animatorController;
    [SerializeField] private SerializedDictionary<string, SkillData> skills = new SerializedDictionary<string, SkillData>();

    [Header("Skill Detector and Hitters")]
    [SerializeField] private SerializedDictionary<SkillData, ObjPooler> skillDetectors = new SerializedDictionary<SkillData, ObjPooler>();
    [SerializeField] private SerializedDictionary<SkillData, ObjPooler> skillHitters = new SerializedDictionary<SkillData, ObjPooler>();
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

    void Start()
    {

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

    public void MakeSkillDetector(int count)
    {
        foreach (var skillData in skills.Values)
        {
            skillDetectors.Add(skillData, ObjPoolerManager.Instance.GetPooler(skillData.SkillTargetDetectorObj));
            skillHitters.Add(skillData, ObjPoolerManager.Instance.GetPooler(skillData.SkillTargetHitterObj));

            var detectorPooler = skillDetectors[skillData];
            var coolTimeRequirement = skillData.requirementDataManager.RequirementDatas.Find(x => x.requirementType == SkillRequirementData.RequirementType.CoolTime);
            var spareCount = coolTimeRequirement == null ? 5 : 1;
            detectorPooler.minimumMaintenanceCost = (int)math.round(count * spareCount * 0.75f);
            detectorPooler.MakeNewOne(detectorPooler.minimumMaintenanceCost);

            var hitterPooler = skillHitters[skillData];
            hitterPooler.minimumMaintenanceCost = detectorPooler.minimumMaintenanceCost;
            hitterPooler.MakeNewOne(hitterPooler.minimumMaintenanceCost);
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
