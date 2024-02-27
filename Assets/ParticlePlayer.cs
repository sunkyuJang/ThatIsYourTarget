using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ParticlePlayer : MonoBehaviour
{
    [SerializeField] private float longestTime = 0f;
    [SerializeField] private ParticleSystem longestParticle;

    public float GetDuration { get => longestTime; }
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void PlayParticle()
    {
        gameObject.SetActive(true);
        TimeCounter.Instance.SetTimeCounting(longestTime, () => { gameObject.SetActive(false); });
    }
    public void GetRemainingTime()
    {
        ParticleSystem particle = null;
        float time = 0f;
        foreach (var particleSystem in transform.GetComponentsInChildren<ParticleSystem>())
        {
            if (particle == null)
            {
                particle = particleSystem;
                time = particle.main.duration;
            }
            else
            {
                if (particleSystem.main.duration > time)
                {
                    time = particleSystem.main.duration;
                    particle = particleSystem;
                }
            }
        }

        longestTime = time;
        longestParticle = particle;
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
            _particlePlayer.GetRemainingTime();
        }
    }
}