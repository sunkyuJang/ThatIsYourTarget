using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillDetectorPoint : MonoBehaviour
{
    private SkillDetectorPointData skillDetectorPointData;
    [SerializeField] private float startDelay = 0f;
    [SerializeField] protected bool shouldIncludeUser = false;
    [SerializeField] protected bool isMultipleTarget = false;
    [SerializeField] protected string targetTag = "";
    [SerializeField] private SkillDetectorPoint nextSkillDetectorStartPoint;

    public void StartDetection(Action<Transform> whenDetected, Transform user)
    {
        if (skillDetectorPointData.doingStartDetetion != null)
            StopCoroutine(skillDetectorPointData.doingStartDetetion);

        skillDetectorPointData = new SkillDetectorPointData
        {
            userTransfrom = user,
            doingStartDetetion = StartCoroutine(DoStartDetection(whenDetected))
        };
    }

    private IEnumerator DoStartDetection(Action<Transform> whenDetected)
    {
        yield return new WaitForSeconds(startDelay);
        OnStartDection(whenDetected);
    }

    protected bool CanAddTarget(Transform target)
    {
        if (!target.CompareTag(targetTag)) return false;

        if (!shouldIncludeUser && !skillDetectorPointData.isAlreadyFindUser)
        {
            if (target.Equals(skillDetectorPointData.userTransfrom))
            {
                skillDetectorPointData.isAlreadyFindUser = true;
                return false;
            }
        }

        return true;
    }

    protected abstract void OnStartDection(Action<Transform> whenDetected);
}

internal class SkillDetectorPointData
{
    public bool isAlreadyFindUser = false;
    public Transform userTransfrom = null;
    public Coroutine doingStartDetetion;
}