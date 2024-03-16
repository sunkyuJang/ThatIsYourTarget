using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JExtentioner;
using System.Linq;
public class SkillDetectorPoint_Ray : SkillDetectorPoint
{
    public float dist = 0f;
    public float distLimit = 0f;
    public float noiseStart = 100f;
    public float noiseRate = 0f;

    protected override void OnStartDection(Action<List<RaycastHit>> whenDetected)
    {
        var targets = transform.GetAllRayHIts(transform.forward, distLimit).ToList();
        targets.OrderBy(x => x.distance);

        var list = new List<RaycastHit>();
        for (int i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            if (CanAddTarget(target.transform))
            {
                list.Add(target);
                if (!isMultipleTarget) break;
            }
        }

        list.ForEach(x => whenDetected?.Invoke(targets));
    }
}

[CustomEditor(typeof(SkillDetectorPoint_Ray))]
public class SkillDetectorPointRayEditor : Editor
{
    // 에디터에서 각 프레임마다 호출되는 함수
    void OnSceneGUI()
    {
        // 타겟 오브젝트를 SkillDetectorPoint_Ray 형태로 가져옵니다.
        SkillDetectorPoint_Ray detector = (SkillDetectorPoint_Ray)target;

        // 시작점을 정의합니다. (SkillDetectorPoint_Ray의 위치)
        Vector3 startPosition = detector.transform.position;

        // 끝점을 정의합니다. (시작점에서 전방으로 distLimit만큼 떨어진 위치)
        Vector3 endPosition = startPosition + detector.transform.forward * detector.distLimit;

        // 레이를 그립니다. 여기서는 라인으로 간단히 표현하였습니다.
        Handles.color = Color.red; // 기즈모 색상을 빨간색으로 설정
        Handles.DrawLine(startPosition, endPosition);

        // 원하는 추가적인 기즈모를 그릴 수 있습니다.
        // 예: 노이즈 시작점 표시
        Vector3 noiseStartPoint = endPosition + detector.transform.forward * detector.noiseStart;
        Handles.color = Color.yellow; // 기즈모 색상을 노란색으로 설정
        Handles.DrawWireDisc(noiseStartPoint, detector.transform.forward, 0.5f); // 반경 0.5의 원을 그립니다.
    }
}