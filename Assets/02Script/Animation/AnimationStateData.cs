using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using JExtentioner;
using UnityEngine.Rendering;

[Serializable]
public class AnimationStateDic
{
    public SerializedDictionary<string, AnimationStateInfo> StateWithTime = new SerializedDictionary<string, AnimationStateInfo>();
}
[Serializable]
public class AnimationStateInfo
{
    [SerializeField] private string name = "";
    [SerializeField] private float length = 0;
    [SerializeField] private string eventsTiming = "";
    [SerializeField] private string exitTime = "";
    [SerializeField] private string nextAnimation = "";

    public float Length { get => length; }
    public List<float> EventsTiming
    {
        get
        {
            var timingsWithString = eventsTiming.Split(',');
            if (timingsWithString.Length > 0)
            {
                var list = new List<float>();
                foreach (var timing in timingsWithString)
                {
                    if (float.TryParse(timing, out float value))
                    {
                        list.Add(value);
                    }
                }

                return list;
            }

            return null;
        }
    }
    public List<KeyValuePair<float, string>> ExitTime
    {
        get
        {
            var timingsWithString = exitTime.Split(',');
            var nextAnimationString = nextAnimation.Split(',');
            if (timingsWithString.Length > 0)
            {
                var list = new List<KeyValuePair<float, string>>();
                var count = 0;
                foreach (var timing in timingsWithString)
                {
                    if (float.TryParse(timing, out float value))
                    {
                        list.Add(new KeyValuePair<float, string>(value, nextAnimationString[count]));
                    }
                    count++;
                }

                return list;
            }

            return null;
        }
    }
    public AnimationStateInfo(string name, float length, List<float> events, List<AnimatorStateTransition> animatorTransitions)
    {
        this.name = name;
        this.length = length;
        events.ForEach(x => eventsTiming += x.Round(3) + ",");
        animatorTransitions.ForEach(x =>
        {
            if (x.exitTime != 0f)
            {
                if (x.destinationState != null)
                {
                    exitTime += x.exitTime.Round(3) + ",";
                    nextAnimation += x.destinationState.name + ",";
                }
            }
        });
    }
}

[Serializable]
public class AnimationStateNode
{
    public bool isSubState;
    public string nowAnimation;
    public string beforeAnimation;
    public SerializedDictionary<string, RequirementDataManager> nextAnimations = new SerializedDictionary<string, RequirementDataManager>();
    public bool hasLoop;
    public SkillData skillData;
    [HideInInspector] public int loopIndex;
    [HideInInspector] public int loopCount = 0;
    [HideInInspector] public int maxLoop = 0;
    [HideInInspector] public Action<Animator> WhenReadAPForSetParametter { set; get; }
    public AnimationStateNode(bool isSubState, string nowAnimation, List<string> nextAnimations)
    {
        this.isSubState = isSubState;
        this.nowAnimation = nowAnimation;
        nextAnimations.ForEach(x => this.nextAnimations.Add(x, new RequirementDataManager()));
        hasLoop = nextAnimations.Find(x =>
        {
            loopIndex++;
            return x == nowAnimation;
        }) != null;
        loopIndex--;
    }
    public AnimationStateNode(AnimationStateNode node)
    {
        this.isSubState = node.isSubState;
        this.nowAnimation = node.nowAnimation;
        this.nextAnimations = node.nextAnimations;
        this.hasLoop = node.hasLoop;
        this.skillData = node.skillData;
    }
}

public static class AnimationComboStateNodeExtentiner
{
    public static AnimationStateNode Copy(this AnimationStateNode node)
    {
        return new AnimationStateNode(node);
    }
}