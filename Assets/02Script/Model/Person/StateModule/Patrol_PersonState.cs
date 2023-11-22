using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JExtentioner;
using System.Linq;
// the goal of patrol is, person should be trace the way that player gone.
public class Patrol_PersonState : PersonState
{
    enum State { tracingTarget, lookAround, done }
    const float castDist = 5.0f;
    JobManager jobManager;
    public Patrol_PersonState(Person person) : base(person) { }

    public override bool IsReady()
    {
        return prepareData != null;
    }
    public override void EnterToException() { }
    protected override void StartModule()
    {
        var objForSection = new object();
        jobManager = new JobManager(objForSection, DoneJobManager);
        SetJobsAction(jobManager);
        jobManager.StartJob();
    }

    protected override void AfterAPHDone()
    {
        jobManager.NextJob();
    }

    void SetJobsAction(JobManager jobManager)
    {
        for (State i = 0; i < State.done; i++)
        {
            var job = new Job(jobManager);
            switch (i)
            {
                case State.tracingTarget:
                    job.jobAction = () => { TracingTarget(); };
                    break;
                case State.lookAround:
                    job.jobAction = () => { LookAroundNearBy(); };
                    break;
            }

            jobManager.AddJob(job);
        }
    }

    void DoneJobManager()
    {
        SetNormalState();
    }

    void TracingTarget()
    {
        var position = GetAroundPositionCast(-30f, 30f, 60f, true);
        var aph = GetAPHByPositions(position);
        SetAPH(aph, true);

        StartCoroutine(DoTracingTarget(aph));
    }
    void LookAroundNearBy()
    {
        var positions = GetAroundPositionCast(-160f, 80f, 160f, false);
        positions.Shuffle();
        var aph = GetAPHByPositions(positions);
        SetAPH(aph, true);

        StartCoroutine(DoTracingTarget(aph));
    }

    IEnumerator DoTracingTarget(AnimationPointHandler aph)
    {
        while (!aph.isAPHDone)
        {
            var isInSight = IsInSight(prepareData.target);
            if (isInSight)
            {
                SetState(StateKinds.Tracking, new PersonPrepareData(prepareData.target));
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        yield break;
    }

    AnimationPointHandler GetAPHByPositions(List<Vector3> positions)
    {
        if (positions.Count <= 0) return null;

        var aph = GetNewAPH(positions.Count, AnimationPointHandler.WalkingState.Walk);
        var count = 0;
        positions.ForEach(x =>
        {
            var ap = aph.GetActionPoint(count++);
            SetAPs(ap, x, PersonAniState.StateKind.LookAround, 0f, true, true, true);
        });

        return aph;
    }

    List<Vector3> GetAroundPositionCast(float startAngle, float angleUnit, float maxAngle, bool onlyFarOne)
    {
        var hitList = new List<Vector3>();
        startAngle = startAngle < 0 ? startAngle : startAngle * -1f;
        for (float angle = startAngle; angle < maxAngle; angle += angleUnit)
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

        if (onlyFarOne)
        {
            var mostFarAway = Vector3.zero;
            var farDist = 0f;
            hitList.ForEach(x =>
            {
                Debug.DrawLine(ActorTransform.position, x, Color.blue, 2f);
                var dist = Vector3.Distance(ActorTransform.position, x);
                if (dist > farDist)
                {
                    farDist = dist;
                    mostFarAway = x;
                }
            });

            return new List<Vector3>() { mostFarAway };
        }
        else
        {
            return hitList;
        }
    }

    Ray GetRay(float angle)
    {
        var rotatedDirection = Quaternion.Euler(0, angle, 0) * ActorTransform.forward;
        return new Ray(ActorTransform.position, rotatedDirection);
    }
    public override void Exit()
    {
        jobManager.CancleJob();
        jobManager = null;
        base.Exit();
    }
}
