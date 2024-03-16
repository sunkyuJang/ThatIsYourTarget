using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JExtentioner;
using System.Linq;
// the goal of patrol is, person should be trace the way that player gone.
public class Patrol_HumanState : HumanState
{
    enum State { tracingTarget, lookAround, done }
    const float castDist = 5.0f;
    JobManager jobManager;
    public Patrol_HumanState(Human person) : base(person) { }

    public override bool IsReady()
    {
        return prepareData != null;
    }
    public override void EnterToException() { }
    protected override void OnStartModule()
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
        var position = prepareData.lastDetectedStandPosition;
        var aph = GetAPHByPositions(new List<Vector3>() { position });
        SetAPH(aph, true);

        StartTracingTargetInSight(prepareData.target, () => aph.isAPHDone);
    }
    void LookAroundNearBy()
    {
        var positions = GetAroundPositionCast(-160f, 80f, 160f, false);
        positions.Shuffle();
        var aph = GetAPHByPositions(positions);
        SetAPH(aph, true);

        StartTracingTargetInSight(prepareData.target, () => aph.isAPHDone);
    }
    protected override bool ShouldStopAfterCast(bool isHit)
    {
        if (isHit)
        {
            SetState(StateKinds.Tracking, new PersonPrepareData(prepareData.target));
            return true;
        }

        return false;
    }
    AnimationPointHandler GetAPHByPositions(List<Vector3> positions)
    {
        if (positions.Count <= 0) return null;

        var aph = GetNewAPH(positions.Count, AnimationPointHandler.WalkingState.Walk);
        var count = 0;
        positions.ForEach(x =>
        {
            var ap = aph.GetAnimationPoint(count++);
            SetAPs(ap, x, HumanAniState.StateKind.LookAround, 0f, true, true);
        });

        return aph;
    }
    List<Vector3> GetAroundPositionCast(float startAngle, float angleUnit, float maxAngle, bool onlyFarOne)
    {
        var hitList = new List<Vector3>();
        startAngle = startAngle < 0 ? startAngle : startAngle * -1f;
        for (float angle = startAngle; angle < maxAngle; angle += angleUnit)
        {
            var dir = Quaternion.Euler(0f, angle, 0f) * ActorTransform.forward;
            var dist = Vector3.Distance(prepareData.target.position, ActorTransform.position);
            hitList.Add(dir * dist);
            GizmosDrawer.instanse.DrawLine(ActorTransform.position, dir * dist, 2f, Color.black);
        }

        return hitList;
    }
    public override void Exit()
    {
        jobManager.CancleJob();
        jobManager = null;
        base.Exit();
    }
}
