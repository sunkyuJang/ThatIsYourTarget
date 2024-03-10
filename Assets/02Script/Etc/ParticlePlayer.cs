using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Rendering.Universal;
using Unity.Mathematics;

public class ParticlePlayer : MonoBehaviour
{
    [SerializeField] private float longestTime = 0f;
    [SerializeField] private ParticleSystem longestParticle;
    [SerializeField] private AudioSource audioSource;

    [Header("Decal Setting")]
    [SerializeField] private DecalProjector decalProjector;
    private enum DecalShowUpSetting { Static, Fade, }
    [SerializeField] private DecalShowUpSetting decalShowUpSetting = DecalShowUpSetting.Static;
    [Range(0f, 1f)][SerializeField] private float showUpStartNormal = 0f;
    [Range(0f, 1f)][SerializeField] private float showUpEndNormal = 0f;
    [SerializeField] private DecalShowUpSetting decalShowOffSetting = DecalShowUpSetting.Static;
    [Range(0f, 1f)][SerializeField] private float showOffStartNormal = 0f;
    [Range(0f, 1f)][SerializeField] private float showOffEndNormal = 1f;
    public float GetDuration { get => longestTime; }

    Coroutine proc_DecalShowUp = null;
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void PlayParticle(Action whenAnimationEnd = null)
    {
        gameObject.SetActive(true);

        if (proc_DecalShowUp != null)
            StopCoroutine(proc_DecalShowUp);

        audioSource?.Play();
        proc_DecalShowUp = StartCoroutine(DecalShowUp());
        TimeCounter.Instance.SetTimeCounting(longestTime, () => { gameObject.SetActive(false); whenAnimationEnd?.Invoke(); });
    }

    IEnumerator DecalShowUp()
    {
        var startShowUpOver = false;
        showUpEndNormal = decalShowUpSetting == DecalShowUpSetting.Static ? showUpStartNormal : showUpEndNormal;
        showOffStartNormal = decalShowOffSetting == DecalShowUpSetting.Static ? showOffStartNormal : showOffEndNormal;
        for (float time = 0; time < longestTime; time += Time.fixedDeltaTime)
        {
            var timeRate = Mathf.InverseLerp(0f, longestTime, time);
            if (!startShowUpOver)
            {
                if (timeRate > showUpStartNormal)
                {
                    var rate = Mathf.InverseLerp(showUpStartNormal, showUpEndNormal, timeRate);
                    decalProjector.fadeFactor = rate;
                }
            }
            else
            {
                if (timeRate > showOffStartNormal)
                {
                    var rate = 1 - Mathf.InverseLerp(showOffStartNormal, showOffEndNormal, timeRate);
                    decalProjector.fadeFactor = rate;
                }
            }
        }

        proc_DecalShowUp = null;
        yield break;
    }
    public void SetDetails()
    {
        ParticleSystem targetParticle = null;
        float time = 0f;
        foreach (var particleSystem in transform.GetComponentsInChildren<ParticleSystem>())
        {
            var targetTime = particleSystem.main.duration > particleSystem.main.startLifetime.constant ? particleSystem.main.duration : particleSystem.main.startLifetime.constant;
            if (targetParticle == null)
            {
                targetParticle = particleSystem;
                time = targetTime;
            }
            else
            {
                if (targetTime > time)
                {
                    time = targetTime;
                    targetParticle = particleSystem;
                }
            }
        }

        audioSource = transform.GetComponentInChildren<AudioSource>();
        decalProjector = transform.GetComponentInChildren<DecalProjector>();

        longestTime = time;
        longestParticle = targetParticle;

        decalProjector = transform.GetComponentInChildren<DecalProjector>();
    }
}


[CustomEditor(typeof(ParticlePlayer))]
public class ParticlePlayerEditor : Editor
{
    private ParticlePlayer _particlePlayer;

    public override void OnInspectorGUI()
    {
        _particlePlayer = (ParticlePlayer)target;
        base.OnInspectorGUI();

        // 버튼 추가
        if (GUILayout.Button("Set Remaining Time"))
        {
            _particlePlayer.SetDetails();
        }
    }
}