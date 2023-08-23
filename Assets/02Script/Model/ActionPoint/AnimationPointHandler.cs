using System.Collections.Generic;
using UnityEngine;

public class AnimationPointHandler : MonoBehaviour
{
    public enum WalkingState { Non, Walk, Run }
    public WalkingState walkingState = WalkingState.Walk;
    public List<AnimationPoint> animationPoints { set; get; } = new List<AnimationPoint>();
    public int GetActionCount { get { return animationPoints.Count; } }
    public int index = 0;
    public bool shouldLoop = true;
    public bool isAPHDone { get { return !shouldLoop && index >= GetActionCount; } }
    public void Awake()
    {
        SetAPs();
    }
    public void SetAPs(List<AnimationPoint> list = null)
    {
        if (list != null)
        {
            list.ForEach(x => x.transform.SetParent(transform));
        }
        SetActionPoint();
    }

    void SetActionPoint()
    {
        animationPoints.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            animationPoints.Add(transform.GetChild(i).GetComponent<AnimationPoint>());
        }
        ResetIndex();
    }

    public AnimationPoint GetNowActionPoint() { return isAPHDone ? null : animationPoints[index]; }

    public AnimationPoint GetNextActionPoint()
    {
        ++index;
        if (shouldLoop)
            index %= GetActionCount;
        else if (isAPHDone)
            return null;

        return GetActionPoint(index);
    }

    public AnimationPoint GetActionPoint(int index)
    {
        var ap = animationPoints[index];
        return ap;
    }

    public AnimationPoint GetEndActionPoint
    {
        get { return animationPoints[animationPoints.Count - 1]; }
    }

    public void ResetIndex() => index = 0;

    public void ChangeAPPositionAndLookAt(int index, Vector3 from, Vector3 to)
    {
        animationPoints[index].ChangePosition(to);
        animationPoints[index].MakeLookAtTo(to);
    }

    public void ResetData()
    {
        ResetIndex();
        shouldLoop = true;

    }
}
