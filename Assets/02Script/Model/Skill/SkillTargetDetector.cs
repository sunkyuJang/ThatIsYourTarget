using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using JExtentioner;
using System.Linq;
public class SkillTargetDetector : MonoBehaviour, IPoolerConnector
{
    public enum HitMotionType { Ray, Cart, Non };
    //private HitMotionType hitMotionType = HitMotionType.Non;
    public SkillDetectorPoint skillDetecterPoint;

    public void StartDetection(Vector3 position, Vector3 forward, Transform user, Action<List<RaycastHit>> whenDetected, Action whenDone)
    {
        transform.position = position;
        transform.forward = forward;

        skillDetecterPoint.StartDetection(whenDetected, user, whenDone);
    }

    public void WhenRetrieveFromPooler()
    {
        gameObject.SetActive(true);
    }

    public void WhenStoreToPooler()
    {
        gameObject.SetActive(false);
    }
}