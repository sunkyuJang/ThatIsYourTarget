using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillTargetHitter : MonoBehaviour, IPoolerConnector
{
    [SerializeField] protected ParticlePlayer basicParticlePlayer;

    protected Data data { set; get; } = null;

    public void WhenRestore()
    {
        data = null;
    }

    public virtual void StartEffect(Vector3 point, Vector3 forward, Action whenDone, Transform dependingOn = null)
    {
        if (dependingOn != null)
            transform.SetParent(dependingOn);

        data = new Data()
        {
            position = point,
            direction = forward,
            TargetTransformData = dependingOn
        };

        transform.position = data.position;
        transform.forward = data.direction;

        PlayParticle(whenDone);
    }

    protected abstract void PlayParticle(Action whenDone);

    public void WhenRetrieveFromPooler()
    {
        gameObject.SetActive(true);
    }

    public void WhenStoreToPooler()
    {
        gameObject.SetActive(false);
        data = null;
    }

    public class Data
    {
        public Vector3 position;
        public Vector3 direction;
        public Transform TargetTransformData { set; get; }
        public Action WhenDone { set; get; }
    }
}
