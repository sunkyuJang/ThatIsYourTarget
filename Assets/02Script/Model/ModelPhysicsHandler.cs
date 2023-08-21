using JExtentioner;
using SensorToolkit;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ModelPhysicsHandler : MonoBehaviour, IJobStarter<Model.ModelJob>, IDamageController
{
    public Model Model { private set; get; }
    AnimationPointHandler actionPointHandler { set; get; }
    NaviController naviController { set; get; }
    AniController aniController { set; get; }
    RagDollHandler ragDollHandler { set; get; }
    Model.ModelJob modelJob { set; get; }
    ModelHandlerJobManager jobManager { set; get; }
    [SerializeField]
    private FOVCollider FOVCollider;
    float SightLength { get { return FOVCollider.Length * FOVCollider.transform.lossyScale.x; } }
    private void Awake()
    {
        Model = transform.parent.GetComponent<Model>();

        ragDollHandler = GetComponent<RagDollHandler>();
        naviController = GetComponent<NaviController>();
        aniController = GetComponent<AniController>();

        if (FOVCollider != null)
        {
            Debug.Log("FovCollider is Missing");
        }
    }

    public void StartJob(Model.ModelJob job)
    {
        if (actionPointHandler != null)
        {
            modelJob.returnAPH(actionPointHandler);
        }

        if (jobManager != null)
            jobManager.CancleJob();

        StopJob();

        modelJob = job;
        actionPointHandler = modelJob.aph;
        jobManager = new ModelHandlerJobManager(
                        endJob: EndJob,
                        modelJob: modelJob,
                        naviJobStarter: naviController,
                        aniJobstarter: aniController);
        jobManager.StartJob();
    }
    void EndJob()
    {
        modelJob?.EndJob();
        modelJob = null;
        jobManager = null;
    }

    public void StopJob()
    {
        aniController.StopJob();
        naviController.StopJob();
    }

    public float GetDistTo(Transform target)
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    public RaycastHit[] GetAllRayHIts(Transform target, float dist = 0f)
    {
        var from = transform.position;
        var to = target.position;
        var dir = from.GetDirection(to);
        dist = dist == 0f ? Vector3.Distance(from, to) : dist;

        return Physics.RaycastAll(from, dir, dist, 0, QueryTriggerInteraction.Ignore).OrderBy(x => x.distance).ToArray();
    }

    bool IsHitToTarget(Transform target, float dist = 0f)
    {
        var from = transform.position;
        var to = target.position;
        var dir = from.GetDirection(to);
        dist = dist == 0f ? Vector3.Distance(from, to) : dist;

        Physics.Raycast(from, dir, out RaycastHit hit, dist);
        return hit.transform == target;
    }

    public bool IsInSight(Transform target)
    {
        return IsHitToTarget(target, SightLength);
    }

    public RaycastHit[] GetAllHitInSight(Transform target)
    {
        return GetAllRayHIts(target, SightLength);
    }

    public void SetDead()
    {
        StopJob();
        ragDollHandler.TrunOnRigid(true);
    }

    public bool SetDamage(float damege)
    {
        return ((IDamageController)Model).SetDamage(damege);
    }

    public Coroutine TracingTargetInSight(Transform target, Func<bool> conditionOfEndLoop, Action<bool> whenHit)
    {
        return StartCoroutine(DoTracingTargetInSight(target, conditionOfEndLoop, whenHit));
    }

    protected IEnumerator DoTracingTargetInSight(Transform target, Func<bool> conditionOfEndLoop, Action<bool> whenHit)
    {
        var maxTime = 360f;
        var time = 0f;
        while (time < maxTime && !conditionOfEndLoop())
        {
            var isHit = IsInSight(target);
            if (isHit)
            {
                whenHit?.Invoke(true);
                yield break;
            }

            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        whenHit?.Invoke(false);
        Debug.Log("DoTracingTargetInSight closed by force : its over than " + maxTime + "sec.\n" + "instanceID : " + transform.GetInstanceID());
        yield break;
    }
}