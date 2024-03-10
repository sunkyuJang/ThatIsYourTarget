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
    private HitMotionType hitMotionType = HitMotionType.Non;
    public SkillDetectorPoint skillDetecterPoint;

    private Transform user;
    private bool isUserAlreadyFound = false;

    public void StartDetection(Vector3 position, Vector3 forward, Transform user, Action<Transform> whenDetected)
    {
        this.user = user;
        transform.position = position;
        transform.forward = forward;

        skillDetecterPoint.StartDetection(whenDetected, user);
    }

    public void ResetObj()
    {
        user = null;
        isUserAlreadyFound = false;
    }
}