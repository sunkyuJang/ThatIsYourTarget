using JMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// the goal of patrol is, person should be trace the way that player gone.
public class Patrol_PersonState : PersonState
{
    enum State { tracingTarget, lookAround, done }
    State state = State.tracingTarget;
    private PrepareData prepareData;
    const int castCount = 6;
    const float castDist = 3.0f;
    JobManager jobManager;
    public Patrol_PersonState(Person person) : base(person) { }

    public void SetPrepareData(PrepareData prepareData)
    {
        this.prepareData = prepareData;
    }
    public override bool IsReadyForEnter()
    {
        return prepareData != null;
    }
    public override void EnterToException() { }
    protected override void DoEnter()
    {
        state = State.tracingTarget;
        var objForSection = new object();
        jobManager = new JobManager(objForSection, DoneJobManager); // after jobManager done, this moduler will be end
        SetJobsAction(jobManager);
        jobManager.StartJob();
    }

    protected override StateKinds DoAfterDone()
    {
        jobManager.NextJob();
        return StateKinds.Patrol; // enter to same state will be ignored.
    }

    void SetJobsAction(JobManager jobManager)
    {
        for (State i = 0; i < State.done; i++)
        {
            var job = new Job(jobManager);
            switch (i)
            {
                case State.tracingTarget:
                    job.jobAction = () => { TracingTarget(job); };
                    break;
                case State.lookAround:
                    job.jobAction = () => { LookAroundNearBy(job); };
                    break;
            }
        }
    }

    void DoneJobManager()
    {
        // if job manager come here means like,
        // in the patrol state, the person didnt find anything.
        // so, it should go to normal state.
        SetState(StateKinds.Normal);
    }

    void TracingTarget(Job job)
    {
        var position = GetForwardPosition();
        var aph = GetAPHByPositions(new List<Vector3>() { position });
        person.SetAPH(aph, AfterAPHDone);

        person.StartCoroutine(DoTracingTarget(job, aph));
    }
    void LookAroundNearBy(Job job)
    {
        var positions = GetRoundPositions();
        var aph = GetAPHByPositions(positions);
        person.SetAPH(aph);
        person.StartCoroutine(DoTracingTarget(job, aph));
    }

    IEnumerator DoTracingTarget(Job job, ActionPointHandler aph)
    {
        var shouldReadNextJob = true;
        while (!aph.isAPHDone)
        {
            var isInSight = person.modelHandler.IsInSight(prepareData.target);
            if (isInSight)
            {
                var trackingPrepareData = new Tracking_PersonState.PrepareData();
                trackingPrepareData.walkingState = ActionPointHandler.WalkingState.Run;
                trackingPrepareData.target = prepareData.target;

                var trackingModule = person.moduleHandler.GetModule<Tracking_PersonState>(StateKinds.Tracking);
                trackingModule.PrepareState(trackingPrepareData);
                shouldReadNextJob = false;
                SetState(StateKinds.Tracking);
            }

            yield return new WaitForFixedUpdate();
        }

        if (shouldReadNextJob)
            job.EndJob();

        yield break;
    }

    ActionPointHandler GetAPHByPositions(List<Vector3> positions)
    {
        if (positions.Count <= 0) return null;

        var aph = person.GetNewAPH(positions.Count, ActionPointHandler.WalkingState.Walk);
        var count = 0;
        positions.ForEach(x =>
        {
            var ap = aph.GetActionPoint(count++);
            person.SetAPs(ap, x, PersonAniState.StateKind.LookAround, true, 0f, true, true);
        });

        return aph;
    }

    Vector3 GetForwardPosition()
    {
        var hitList = new List<Vector3>();
        for (float angle = -30f; angle < 60; angle += 30f)
        {
            var ray = GetRay(angle);
            if (Physics.Raycast(ray, out RaycastHit hit, castDist))
            {
                hitList.Add(hit.point);
            }
            else
            {
                hitList.Add(ray.GetPoint(castDist));
            }
        }

        var path = new NavMeshPath();
        var mostFarAway = Vector3.zero;
        var farDist = 0f;
        hitList.ForEach(x =>
        {
            if (NavMesh.CalculatePath(person.modelHandler.transform.position, x, 1, path))
            {
                var dist = Vector3.Distance(person.modelHandler.transform.position, x);
                if (dist > farDist)
                {
                    farDist = dist;
                    mostFarAway = x;
                }
            }
        });

        return mostFarAway;
    }

    List<Vector3> GetRoundPositions()
    {
        var samplingCount = 10;
        List<Vector3> castList = new List<Vector3>();
        for (int i = 0; i < samplingCount; i++)
        {
            if (NavMesh.SamplePosition(person.modelHandler.transform.position, out NavMeshHit hit, castDist, 1))
            {
                castList.Add(hit.position);
            }
        }

        if (castList.Count > 0)
        {
            castList.Shuffle();
        }

        return castList;
    }

    Ray GetRay(float angle)
    {
        var rad = angle * Mathf.Deg2Rad;
        var dir = new Vector3(Mathf.Cos(rad), 0, Mathf.Sign(rad));
        return new Ray(prepareData.target.position, dir);
    }
    public override void Exit()
    {
        jobManager.CancleJob();
        jobManager = null;
    }
    public class PrepareData
    {
        public Transform target;
    }
}
