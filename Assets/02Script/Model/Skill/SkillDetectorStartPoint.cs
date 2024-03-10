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

    [Header("skill sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float audioDelay = 0f;

    [Header("skill Effect")]
    [SerializeField] private ParticlePlayer particlePlayer;
    [SerializeField] private float effectDelay = 0f;

    [Header("skill Detail")]
    [SerializeField] private float startDelay = 0f;
    [SerializeField] protected bool shouldIncludeUser = false;
    [SerializeField] protected bool isMultipleTarget = false;
    [SerializeField] protected List<string> targetTags = new List<string>();

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
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        TimeCounter.Instance.SetTimeCounting(audioDelay, () => { audioSource.Play(); });
        TimeCounter.Instance.SetTimeCounting(effectDelay, () => { particlePlayer.PlayParticle(); });

        OnStartDection(whenDetected);
    }

    protected bool CanAddTarget(Transform target)
    {
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

    protected abstract void OnStartDection(Action<Transform> whenDetected);
    // protected abstract void OnStartEffect(Action<Transform> whenDetected);
    // protected abstract void OnStartSound(Action<Transform> whenDetected);
    // protected abstract void OnHitEffect(Action<Transform> whenDetected);
    // protected abstract void OnHitSound(Action<Transform> whenDetected);

}
internal class SkillDetectorPointData
{
    public bool isAlreadyFindUser = false;
    public Transform userTransfrom = null;
    public Coroutine doingStartDetetion;
}