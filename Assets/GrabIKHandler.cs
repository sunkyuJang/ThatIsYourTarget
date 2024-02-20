using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RootMotion.FinalIK;
using System.ComponentModel;
using System;
using UnityEngine.Rendering;
[RequireComponent(typeof(LimbIK))]
[RequireComponent(typeof(FingerRig))]
public class HumanHandLimbIKSetter : MonoBehaviour
{
    [ReadOnly] public HumanBodyBones handBone = HumanBodyBones.RightHand;
    [ReadOnly] public LimbIK limbIK;
    [ReadOnly] public FingerRig fingerRig;
    public enum Fingers { finger, index, thumb }
    [SerializeField] private float weight = 0f;
    private Coroutine Proc_TurnIK { set; get; }

    private void Awake()
    {
        //fingerRig.weight = weight;
    }

    public void SetTipToFingerRig()
    {
        limbIK = GetComponent<LimbIK>();
        fingerRig = GetComponent<FingerRig>();
        fingerRig.AutoDetect();

        var animator = GetComponentInParent<Animator>();
        handBone = transform == animator.GetBoneTransform(HumanBodyBones.LeftHand) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand;
    }

    public void TurnOnIK(bool turnOn, float during, Transform handPosition, FingerPositioner fingerPositioner)
    {
        if (Proc_TurnIK != null)
            StopCoroutine(Proc_TurnIK);

        Proc_TurnIK = StartCoroutine(DoTurnOnIK(turnOn, during, handPosition, fingerPositioner));
    }

    IEnumerator DoTurnOnIK(bool turnOn, float during, Transform handPosition, FingerPositioner fingerPositioner)
    {
        float startWeight = weight;
        float endWeight = turnOn ? 1f : 0f;
        float elapsedTime = 0f;

        if (turnOn) // if its turn off, the setting will keep untill weight 0;
        {
            limbIK.solver.target = handPosition;
            if (fingerPositioner != null)
                for (Fingers i = Fingers.finger; i <= Fingers.thumb; i++)
                {
                    var targetFinger = fingerRig.fingers[(int)i];
                    switch (i)
                    {
                        case Fingers.finger: targetFinger.target = fingerPositioner.finger; break;
                        case Fingers.index: targetFinger.target = fingerPositioner.index; break;
                        case Fingers.thumb: targetFinger.target = fingerPositioner.thumb; break;
                    }
                    targetFinger.weight = targetFinger.target == null ? 0 : 1;
                }
        }

        while (elapsedTime <= during)
        {
            elapsedTime += Time.fixedDeltaTime;
            weight = Mathf.Lerp(startWeight, endWeight, elapsedTime / during);

            // Update IK weights here
            limbIK.solver.IKPositionWeight = weight;
            limbIK.solver.IKRotationWeight = weight;

            if (fingerPositioner != null)
            {
                fingerRig.weight = weight;

            }
            yield return new WaitForFixedUpdate();
        }

        if (!turnOn)
        {
            limbIK.solver.target = null;
            for (Fingers i = Fingers.finger; i <= Fingers.thumb; i++)
            {
                var targetFinger = fingerRig.fingers[(int)i];
                switch (i)
                {
                    case Fingers.finger: targetFinger.target = null; break;
                    case Fingers.index: targetFinger.target = null; break;
                    case Fingers.thumb: targetFinger.target = null; break;
                }

                targetFinger.weight = 0;
            }
        }

        Proc_TurnIK = null;
    }
}

[Serializable]
public class GrabPositioner
{
    public Transform aimTransform;
    public GrabPositioner() { }
    public GrabPositioner(Transform aimTransform)
    {
        this.aimTransform = aimTransform;
    }
}
[Serializable]
public class PersonGrabPositioner : GrabPositioner
{
    public FingerPositioner LFingerPositioner = new FingerPositioner();
    public FingerPositioner RFingerPositioner = new FingerPositioner();
    public PersonGrabPositioner() { }
    public PersonGrabPositioner(Transform aimTransform, FingerPositioner L_FingerPositioner, FingerPositioner R_FingerPositioner) : base(aimTransform)
    {
        this.LFingerPositioner = L_FingerPositioner;
        this.RFingerPositioner = R_FingerPositioner;
    }
}
[Serializable]
public class FingerPositioner
{
    public Transform thumb;
    public Transform index;
    public Transform finger;
}

[CustomEditor(typeof(HumanHandLimbIKSetter))]
public class GrabRigPositionerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터 UI를 그립니다.
        base.OnInspectorGUI();

        // GrabRigPositioner 스크립트에 대한 참조를 가져옵니다.
        HumanHandLimbIKSetter grabRigPositioner = (HumanHandLimbIKSetter)target;

        if (grabRigPositioner.limbIK == null || grabRigPositioner.fingerRig == null)
        {
            EditorGUILayout.HelpBox("you must apply limIK and fingerRig. plz press Auto Set button", MessageType.Warning);
        }

        // 버튼을 인스펙터에 추가합니다.
        if (GUILayout.Button("Auto Set"))
        {
            grabRigPositioner.SetTipToFingerRig();
        }
    }
}
