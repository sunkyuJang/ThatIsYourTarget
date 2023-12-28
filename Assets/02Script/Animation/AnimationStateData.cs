using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using JExtentioner;
using UnityEngine.Rendering;
using Unity.VisualScripting;

[Serializable]
public class AnimationStateDic
{
    public SerializedDictionary<string, AnimationStateData> StateWithTime = new SerializedDictionary<string, AnimationStateData>();
}
[Serializable]
public class AnimationStateData
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
    public AnimationStateData(string name, float length, List<float> events, List<AnimatorStateTransition> animatorTransitions)
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
public class AnimationComboStateNode
{
    public bool isSubState;
    public string nowAnimation;
    public List<string> nextAnimations = new List<string>();
    public Func<int> SetCondition = null;
    public bool hasLoop;
    [HideInInspector] public int loopIndex;
    [HideInInspector] public int loopCount = 0;
    [HideInInspector] public int maxLoop = 0;
    [HideInInspector] public Action<Animator> WhenReadAPForSetParametter { set; get; }
    public AnimationComboStateNode(bool isSubState, string nowAnimation, List<string> nextAnimations)
    {
        this.isSubState = isSubState;
        this.nowAnimation = nowAnimation;
        this.nextAnimations = nextAnimations;
        hasLoop = nextAnimations.Find(x =>
        {
            loopIndex++;
            return x == nowAnimation;
        }) != null;
        loopIndex--;
    }
    public string ReadNextAnimation()
    {
        var index = SetCondition?.Invoke() ?? null;
        if (index == null) return null;
        if (index.Value < 0 || index.Value >= nextAnimations.Count) return null;
        if (hasLoop && index.Value == loopIndex)
            loopCount++;

        return nextAnimations[index.Value];
    }

    public string ReadNextAnimation(int index)
    {
        if (0 <= index && index < nextAnimations.Count) return nextAnimations[index];
        return null;
    }
}

public static class AnimationComboStateNodeExtentiner
{
    public static AnimationComboStateNode Copy(this AnimationComboStateNode node)
    {
        return new AnimationComboStateNode(node.isSubState, node.nowAnimation, node.nextAnimations);
    }
}