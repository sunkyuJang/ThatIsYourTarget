using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using JExtentioner;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NavMeshObstacle))]
public class NaviController : MonoBehaviour, IJobStarter<ModelAnimationPlayerJobManager.ModelHandlerJob>
{
    public enum State { Reached, Near, Close, MiddleWay }
    public NavMeshAgent navMeshAgent { private set; get; }
    NavMeshObstacle navMeshObstacle { set; get; }
    Coroutine CheckingUntilArrive;
    public AnimationPoint playingAP { private set; get; }
    readonly public static List<KeyValuePair<State, float>> eachStateDist = new List<KeyValuePair<State, float>>()
    {
        new KeyValuePair<State, float>(State.Reached, 0.01f),
        new KeyValuePair<State, float>(State.Near, 1f),
        new KeyValuePair<State, float>(State.Close, 2f),
    };
    float GetDistByState(State state)
    {
        foreach (var item in eachStateDist)
        {
            if (item.Key.Equals(state)) return item.Value;
        }

        return -1f;
    }
    State GetStateByDist
    {
        get
        {
            var distXZ = Vector2.Distance(transform.position.ConvertVector3To2(1), navMeshAgent.destination.ConvertVector3To2(1));
            for (int i = 0; i < eachStateDist.Count; i++)
            {
                var pair = eachStateDist[i];
                if (distXZ <= pair.Value)
                    return pair.Key;
            }

            return State.MiddleWay;
        }
    }

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        navMeshObstacle.enabled = false;
    }

    private void Start()
    {
        navMeshAgent.avoidancePriority = NaviTrafficManager.Instance.NaviAvoidance++;
        TurnOnNavi(true);
    }
    public void StartJob(ModelAnimationPlayerJobManager.ModelHandlerJob job)
    {
        if (job.ap != null)
        {
            if (job.ap != null)
                Debug.Log(job.ap.animationPointData.state);

            if (job.ap == playingAP) // looping
                return;

            if (CheckingUntilArrive != null)
            {
                StopCoroutine(CheckingUntilArrive);
            }

            var correctVector = job.ap.transform.position;
            navMeshAgent.avoidancePriority = 0;
            playingAP = job.ap;
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            var needNavi = Vector3.Distance(job.ap.transform.position, transform.position) > eachStateDist[(int)State.Reached].Value;
            if (needNavi)
            {
                navMeshAgent.stoppingDistance = eachStateDist[(int)State.Reached].Value;
                CheckingUntilArrive = StartCoroutine(DoCheckUntilArrive(job, correctVector));
            }
            else
            {
                TurnOnNavi(false);
                navMeshAgent.stoppingDistance = 99f;
                job.EndJob();
            }
        }
    }

    IEnumerator DoCheckUntilArrive(ModelAnimationPlayerJobManager.ModelHandlerJob job, Vector3 correctedVector)
    {
        var jobEnded = false;
        var loopEndByAniDone = false;
        if (job.ap.animationPointData.ShouldTrackingTarget)
        {
            if (job.ap.animationPointData.LookAtTransform != null)
            {
                job.EndJob();
                jobEnded = true;
                job.ap.animationPointData.whenAnimationEnd += () => { loopEndByAniDone = true; };
                navMeshAgent.stoppingDistance = job.ap.animationPointData.StoppingDistance;
            }
        }

        TurnOnNavi(true);
        var ap = job.ap;
        ap.animationPointData.CorrectedPosition = correctedVector;

        var lastPosition = ap.transform.position;
        TimeCounter.TimeCountData crowededTimeData = null;
        TimeCounter.TimeCountData deadLockTimeData = null;
        MetaphysicsTrafficHandler.TrafficData trafficData = null;

        var hasBeenCheckNearBy = false;

        SetDestination(ap.transform.position, ref correctedVector);
        while (GetStateByDist != State.Reached && !loopEndByAniDone)
        {
            var state = GetStateByDist;
            if (lastPosition != ap.transform.position)
            {
                lastPosition = ap.transform.position;
                SetDestination(ap.transform.position, ref correctedVector);
                trafficData?.RemoveControllers(this);
                hasBeenCheckNearBy = false;
            }

            // can it yield when it get close to AP? 
            if (state == State.Close || state == State.Near)
            {
                if (!hasBeenCheckNearBy)
                {
                    hasBeenCheckNearBy = true;
                    if (ap.animationPointData.CanYield)
                    {
                        if (!NaviTrafficManager.Instance.IsCongested(correctedVector, this, out trafficData))
                        {
                            trafficData.AddingControllers(this, ap, correctedVector);
                            NaviTrafficManager.Instance.AddTrafficPoint(trafficData);
                        }
                    }
                    else
                    {
                        if (IsCrowededByNearObj(out Collider[] hitCollider))
                        {
                            if (ap.ShouldPlaySamePosition || hitCollider.Length > 1)
                                navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                        }
                    }
                }
            }

            // checking Croweded
            if (crowededTimeData == null)
            {
                crowededTimeData = TimeCounter.Instance.SetTimeCounting(
                    maxTime: 0.5f,
                    function: () =>
                        {
                            navMeshAgent.obstacleAvoidanceType = IsCrowededByNearObj(out Collider[] hitCollider) ? ObstacleAvoidanceType.NoObstacleAvoidance : ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                            crowededTimeData = null;
                        },
                    sequenceKey: playingAP,
                    sequnceMatch: (addedSeqeunce) => addedSeqeunce.Equals(playingAP));
            }

            // checking DeadLock
            if (deadLockTimeData == null)
            {
                var eachTime = 0.1f;
                var lastStanding = transform.position;
                var speedCorrection = 0.8f;
                var expectingDist = navMeshAgent.velocity.magnitude * speedCorrection * eachTime;
                deadLockTimeData = TimeCounter.Instance.SetTimeCounting(
                    maxTime: eachTime,
                    function: () =>
                        {
                            var dist = Vector3.Distance(transform.position, lastStanding);
                            if (dist < expectingDist)
                            {
                                navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                            }

                            deadLockTimeData = null;
                        },
                    sequenceKey: playingAP,
                    sequnceMatch: (addedSeqeunce) => addedSeqeunce.Equals(playingAP));
            }

            yield return new WaitForFixedUpdate();
        }

        navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        ap.animationPointData.CorrectedPosition = correctedVector;
        navMeshAgent.avoidancePriority = 99;
        navMeshAgent.stoppingDistance = 100f;
        TurnOnNavi(false);

        playingAP = null;

        if (!jobEnded)
            job.EndJob();
    }

    bool IsCrowededByNearObj(out Collider[] hitColliders)
    {
        var isCroweded = false;
        var distByState = GetDistByState(State.Close);
        GizmosDrawer.instanse.DrawSphere(transform.position, distByState, 2f, Color.red - new Color(0, 0, 0, 0.7f));
        hitColliders = Physics.OverlapSphere(transform.position, distByState, LayerMask.GetMask("Actor"));
        var count = 0;
        var maxCount = 3;
        foreach (var collider in hitColliders)
        {
            count++;

            if (count > maxCount)
            {
                isCroweded = true;
                break;
            }
        }

        return isCroweded;
    }
    void SetDestination(Vector3 targetPosition, ref Vector3 correctiondVector)
    {
        if (IsPositionCanReach(targetPosition, out NavMeshHit hit))
        {
            correctiondVector = hit.position;
            navMeshAgent.SetDestination(correctiondVector);
            GizmosDrawer.instanse.DrawLine(transform.position, correctiondVector, 2f, Color.magenta);
            GizmosDrawer.instanse.DrawSphere(correctiondVector, 0.05f, 2f, Color.magenta);
        }
    }
    bool IsPositionCanReach(Vector3 targetPosition, out NavMeshHit hit)
    {
        hit = new NavMeshHit();
        var filter = new NavMeshQueryFilter
        {
            agentTypeID = navMeshAgent.agentTypeID,
            areaMask = NavMesh.AllAreas
        };

        for (float searchRadius = 5f, increasingRadius = 10f; searchRadius < 50f; searchRadius += increasingRadius)
        {
            if (NavMesh.SamplePosition(targetPosition, out hit, searchRadius, filter))
            {
                return true;
            }
        }

        return false;
    }
    public void StopJob()
    {
        if (CheckingUntilArrive != null)
            StopCoroutine(CheckingUntilArrive);
    }

    public void TurnOnNavi(bool shouldTurnOn)
    {
        //dont change the sequence.
        if (shouldTurnOn)
        {
            navMeshObstacle.enabled = !shouldTurnOn;
            navMeshAgent.enabled = shouldTurnOn;
        }
        else
        {
            navMeshAgent.enabled = shouldTurnOn;
            navMeshObstacle.enabled = !shouldTurnOn;
        }
    }

    public Vector3 GetNaviDirection()
    {
        var direction = Vector3.zero;
        if (navMeshAgent != null)
        {
            if (navMeshAgent.isOnNavMesh
                    && !navMeshAgent.isStopped)
            {
                direction = navMeshAgent.velocity.normalized;
            }
        }

        return direction;
    }
}