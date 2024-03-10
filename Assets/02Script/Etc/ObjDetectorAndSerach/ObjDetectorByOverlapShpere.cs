using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ObjDetectorByOverlapShpere : ObjDetector
{
    [SerializeField] private float radius = 1f;
    [SerializeField] private bool isCastOnEnable = false;
    private void OnEnable()
    {
        if (isCastOnEnable)
            Cast();
    }
    private void Cast()
    {
        var hits = Physics.OverlapSphere(transform.position, radius);
        foreach (var item in hits)
        {
            if (IsFind(item.transform))
            {
                I_OnDetected?.OnDetected(this, item.transform);
                break;
            }
        }
    }
}

[CustomEditor(typeof(ObjDetectorByOverlapShpere))]
public class ObjDetectorByOverlapShpereEditor : Editor
{
    // 에디터에서 각 프레임마다 호출되는 함수
    void OnSceneGUI()
    {
        serializedObject.Update();
        // 타겟 오브젝트를 SkillDetectorPoint_Ray 형태로 가져옵니다.
        ObjDetectorByOverlapShpere detector = (ObjDetectorByOverlapShpere)target;

        SerializedProperty radiusProp = serializedObject.FindProperty("radius");

        EditorGUI.BeginChangeCheck();
        var radius = EditorGUILayout.FloatField("Radius", radiusProp.floatValue);
        var rotateLeftVector = Vector3.Cross(detector.transform.forward, Vector3.left);
        var rotateUpVector = Vector3.Cross(detector.transform.up, Vector3.up);
        Vector3 startPosition = detector.transform.position;

        Handles.color = Color.yellow; // 기즈모 색상을 노란색으로 설정
        Handles.DrawWireDisc(detector.transform.position, detector.transform.forward, radius);
        Handles.DrawWireDisc(detector.transform.position, rotateLeftVector, radius);
        Handles.DrawWireDisc(detector.transform.position, rotateUpVector, radius);
    }
}