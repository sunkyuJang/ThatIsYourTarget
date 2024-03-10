using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class HumanFingerPositioner : MonoBehaviour
{
    public FingerPositioner RFingerPositioner = new FingerPositioner();
    public FingerPositioner LFingerPositioner = new FingerPositioner();

}

[CustomEditor(typeof(HumanFingerPositioner))]
public class HumanFingerPositionerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var fingerPositioner = target as HumanFingerPositioner;
        if (GUILayout.Button("Auto Load"))
        {
            for (int i = 0; i < 2; i++)
            {
                var handPosition = i == 0 ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand;
                var targetFingerPositioner = handPosition == HumanBodyBones.LeftHand ? fingerPositioner.LFingerPositioner : fingerPositioner.RFingerPositioner;
                var targetTransform = fingerPositioner.transform.Find(handPosition.ToString());

                Transform thumb = null;
                Transform index = null;
                Transform finger = null;
                if (targetTransform == null)
                {
                    targetTransform = new GameObject(handPosition.ToString()).transform;
                    targetTransform.SetParent(fingerPositioner.transform);
                }

                thumb = GetEachFingerPart(targetTransform, HumanHandLimbIKSetter.Fingers.thumb);
                index = GetEachFingerPart(targetTransform, HumanHandLimbIKSetter.Fingers.index);
                finger = GetEachFingerPart(targetTransform, HumanHandLimbIKSetter.Fingers.finger);

                targetFingerPositioner.thumb = thumb.transform;
                targetFingerPositioner.index = index.transform;
                targetFingerPositioner.finger = finger.transform;
            }
        }
    }

    Transform GetEachFingerPart(Transform target, HumanHandLimbIKSetter.Fingers fingers)
    {
        var finger = target.Find(fingers.ToString());
        if (finger == null)
        {
            var targetFinger = new GameObject(fingers.ToString());
            targetFinger.transform.SetParent(target);

            return targetFinger.transform;
        }

        return finger;
    }
}