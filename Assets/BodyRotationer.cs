using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using JExtentioner;

public class BodyRotationer : MonoBehaviour
{
    public float rotationSpeed = 2f;
    private void Start()
    {
        //StartCoroutine(DoTracking());
    }

    // IEnumerator DoTracking()
    // {
    //     yield return new WaitForFixedUpdate();
    //     while (true)
    //     {
    //         if (spineAim.weight != 1) continue;

    //         var spindAimData = spineAim.data;
    //         var limit = spindAimData.limits.y * 0.5f;
    //         var targetDir = spindAimData.sourceObjects[0].transform.position.ExceptVector3Property(1) - transform.position.ExceptVector3Property(1);
    //         var targetAngle = transform.forward.GetRotationDir(targetDir);
    //         if (Mathf.Abs(targetAngle) >= limit)
    //         {
    //             Quaternion targetRotation = Quaternion.LookRotation(targetDir);
    //             transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    //         }

    //     }
    // }
}