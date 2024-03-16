using System.Collections;
using System.Collections.Generic;
using JExtentioner;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.Threading;
using System;

public class ReflectionByMaterial_SkillTargetHitter : SkillTargetHitter
{
    [SerializeField] private SerializedDictionary<MaterialType.MaterialTypeEnum, ParticlePlayer> particlePlayer = new SerializedDictionary<MaterialType.MaterialTypeEnum, ParticlePlayer>();
    protected override void PlayParticle(Action whenDone)
    {
        if (data.TargetTransformData != null
            && data.TargetTransformData.GetComponent<MaterialType>() == null
            && particlePlayer.ContainsKey(data.TargetTransformData.GetComponent<MaterialType>().TypeOfMaterial))
        {
            var materialType = data.TargetTransformData.GetComponent<MaterialType>().TypeOfMaterial;
            particlePlayer[materialType].PlayParticle(whenDone);
        }
        else
        {
            basicParticlePlayer?.PlayParticle(whenDone);
        }
    }

    public void SetParticlePlayer()
    {
        particlePlayer.Clear();
        foreach (var materialType in transform.GetComponentsInChildren<MaterialType>())
        {
            var childParticlePlayer = materialType.gameObject.GetComponent<ParticlePlayer>();
            if (childParticlePlayer == null)
            {
                Debug.Log("particlePlayer is missing");
            }
            else
            {
                particlePlayer[materialType.TypeOfMaterial] = childParticlePlayer; // 새 ParticlePlayer 인스턴스를 생성하여 딕셔너리에 추가
            }
        }
    }
}

[CustomEditor(typeof(ReflectionByMaterial_SkillTargetHitter))]
public class ReflectionByMaterial_SkillTargetHitterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // 기본 인스펙터를 표시

        ReflectionByMaterial_SkillTargetHitter script = target as ReflectionByMaterial_SkillTargetHitter;

        if (GUILayout.Button("Set Particle Player"))
        {
            script.SetParticlePlayer(); // SetParticlePlayer 메서드 호출
        }
    }
}