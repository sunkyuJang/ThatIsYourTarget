using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using JExtentioner;
using System.Collections.Generic;
using UnityEngine.PlayerLoop;
using Unity.VisualScripting;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NavMeshObstacle))]
public class NaviController : MonoBehaviour, IJobStarter<ModelAnimationPlayerJobManager.ModelHandlerJob>
{
    public enum State { Reached, Near, Close, MiddleWay }
    public NavMeshAgent navMeshAgent { private set; get; }
    NavMeshObstacle navMeshObstacle { set; get; }
    Collider Collider { set; get; }
    Coroutine CheckingUntilArrive;
    public AnimationPoint playingAP { private set; get; }
    TimeCounter.TimeCountData TimeCountData { set; get; }
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
        Collider = GetComponent<Collider>();
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
            if (CheckingUntilArrive != null)
                StopCoroutine(CheckingUntilArrive);

            TimeCountData = null;
            var correctVector = job.ap.transform.position;
            navMeshAgent.avoidancePriority = 0;
            playingAP = job.ap;
            var needNavi = Vector3.Distance(job.ap.transform.position, transform.position) > eachStateDist[(int)State.Reached].Value;
            if (needNavi)
            {
                navMeshAgent.stoppingDistance = eachStateDist[(int)State.Reached].Value; ;
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
        TurnOnNavi(true);
        var ap = job.ap;
        ap.CorrectedPosition = correctedVector;

        var lastPosition = ap.transform.position;
        System.Func<bool> IsAPPositionChanged = () => { return lastPosition != ap.transform.position; };
        TimeCounter.TimeCountData timeCountData = null;

        SetDestination(ap.transform.position, ref correctedVector);
        while (GetStateByDist != State.Reached)
        {
            var state = GetStateByDist;
            if (IsAPPositionChanged())
            {
                lastPosition = ap.transform.position;
                SetDestination(ap.transform.position, ref correctedVector);
            }

            if (timeCountData == null)
            {
                TimeCountData = timeCountData = TimeCounter.Instance.SetTimeCounting(0.5f, () =>
                {
                    if (timeCountData == null || TimeCountData == null) return;
                    if (!TimeCountData.Equals(timeCountData)) return;

                    navMeshAgent.obstacleAvoidanceType = IsCrowededNear() ? ObstacleAvoidanceType.NoObstacleAvoidance : ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                    timeCountData = null;
                },
                playingAP,
                (addedSeqeunce) => addedSeqeunce.Equals(playingAP));
            }

            yield return new WaitForFixedUpdate();
        }

        ap.CorrectedPosition = correctedVector;
        navMeshAgent.avoidancePriority = 99;
        navMeshAgent.stoppingDistance = 100f;
        TurnOnNavi(false);

        playingAP = null;
        job.EndJob();
    }

    bool IsCrowededNear()
    {
        var isCroweded = false;
        var distByState = GetDistByState(State.Close);
        GizmosDrawer.instanse.DrawSphere(transform.position, distByState, 2f, Color.red - new Color(0, 0, 0, 0.7f));
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, distByState, LayerMask.GetMask("Actor"));
        var count = 0;
        var maxCount = 4;
        foreach (var collider in hitColliders)
        {
            if (collider.Equals(Collider))
                continue;
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