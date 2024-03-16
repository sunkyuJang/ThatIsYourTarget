using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class SkillDetectorPoint : MonoBehaviour
{
    private SkillDetectorPointData skillDetectorPointData;

    [Header("next chain Skill")]
    [SerializeField] private SkillDetectorPoint nextSkillDetectorStartPoint;
    [SerializeField] private float nextSkillPlayDelay = 0f;

    [Header("skill Effect")]
    [SerializeField] private ParticlePlayer particlePlayer;
    [SerializeField] private float effectDelay = 0f;

    [Header("skill Detail")]
    [SerializeField] private float startDelay = 0f;
    [SerializeField] protected bool shouldIncludeUser = false;
    [SerializeField] protected bool isMultipleTarget = false;
    [SerializeField] protected bool isTargetIgnore = false;
    [SerializeField] protected List<string> targetTags = new List<string>();

    protected Coroutine Proc_StartDectection { set; get; } = null;

    public void StartDetection(Action<List<RaycastHit>> whenDetected, Transform user, Action whenDone)
    {
        if (Proc_StartDectection != null)
            StopCoroutine(Proc_StartDectection);

        skillDetectorPointData = new SkillDetectorPointData
        {
            userTransfrom = user,
        };

        Proc_StartDectection = StartCoroutine(DoStartDetection(whenDetected, whenDone));
    }

    private IEnumerator DoStartDetection(Action<List<RaycastHit>> whenDetected, Action whenDone)
    {
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        TimeCounter.Instance.SetTimeCounting(effectDelay, () => { particlePlayer.PlayParticle(whenDone); });

        OnStartDection(whenDetected);
    }

    protected bool CanAddTarget(Transform target)
    {
        if (!isTargetIgnore)
            if (!targetTags.Contains(target.tag)) return false;

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

    protected abstract void OnStartDection(Action<List<RaycastHit>> whenDetected);
    // protected abstract void OnStartEffect(Action<Transform> whenDetected);
    // protected abstract void OnStartSound(Action<Transform> whenDetected);
    // protected abstract void OnHitEffect(Action<Transform> whenDetected);
    // protected abstract void OnHitSound(Action<Transform> whenDetected);

}
internal class SkillDetectorPointData
{
    public bool isAlreadyFindUser = false;
    public Transform userTransfrom = null;
}