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
    public SerializedDictionary<string, SkillData> AllSkillData { get => skills; }
    private SerializedDictionary<Model, Dictionary<SkillData, SkillToken>> usingSkill = new SerializedDictionary<Model, Dictionary<SkillData, SkillToken>>();
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
    public void AddSkill(SkillData skillData)
    {
        if (skills.ContainsKey(skillData.keyName))
        {
            Debug.Log("duplicated skill" + skillData.keyName);
        }
        else
        {
            skills.Add(skillData.keyName, skillData);
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

    public class SkillToken
    {
        public SkillData SkillData;
        public Model usingModel;
        public List<Model> targeModel;
        public float timeStemp;
    }
}
