using System.Collections.Generic;
using UnityEditor;
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

[CustomEditor(typeof(AnimationPointHandler))]
public class AnimationPointHandlerEditor : Editor
{
    AnimationPointHandler handler { get; set; }
    List<AnimationPoint> animationPoints = new List<AnimationPoint>();
    public void OnEnable()
    {
        handler = target as AnimationPointHandler;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
    void OnSceneGUI()
    {
        if (handler != null)
        {
            if (handler.transform.childCount != animationPoints.Count)
            {
                animationPoints.Clear();

                for (int i = 0; i < handler.transform.childCount; i++)
                {
                    var ap = handler.transform.GetChild(i).GetComponent<AnimationPoint>();
                    animationPoints.Add(ap);
                }
            }

            for (int i = 0; i < animationPoints.Count; i++)
            {
                Handles.color = Color.red;
                var ap = animationPoints[i];
                var nextAp = i + 1 < animationPoints.Count ? animationPoints[i + 1] : null;
                var startDrawPosition = Vector3.up * 0.1f;
                var endDrawPosition = startDrawPosition + Vector3.up * 0.1f;
                var sphereRadius = 0.05f;
                if (handler.shouldLoop && nextAp == null)
                {
                    nextAp = animationPoints[0];
                }

                if (nextAp)
                {
                    Handles.DrawLine(ap.transform.position, nextAp.transform.position);
                }

                if (i == animationPoints.Count - 1) // �������϶�
                {
                    var drawPoint = handler.shouldLoop ? animationPoints[0] : animationPoints[i];
                    Handles.SphereHandleCap(0, drawPoint.transform.position + endDrawPosition, Quaternion.identity, sphereRadius, EventType.Repaint);
                }

                if (i == 0) // �������ϋ�
                {
                    Handles.color = Color.green;
                    Handles.SphereHandleCap(0, ap.transform.position + startDrawPosition, Quaternion.identity, sphereRadius, EventType.Repaint);
                }
            }
        }
    }
}
