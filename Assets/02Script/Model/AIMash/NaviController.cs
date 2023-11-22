using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using JExtentioner;
using System.Linq;
using UnityEditor.Experimental.GraphView;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NavMeshObstacle))]
public class NaviController : MonoBehaviour, IJobStarter<ModelAnimationPlayerJobManager.ModelHandlerJob>
{
    public NavMeshAgent navMeshAgent { private set; get; }
    NavMeshObstacle navMeshObstacle { set; get; }
    float permissibleRangeToDestinationXZ = 0.01f;
    float alomstArrived = 1f;
    bool IsArrivedDestination
    {
        get
        {
            var distXZ = Vector2.Distance(transform.position.ConvertVector3To2(1), navMeshAgent.destination.ConvertVector3To2(1));
            return distXZ < permissibleRangeToDestinationXZ;
        }
    }

    bool IsAlmostArrivedDestination
    {
        get
        {
            var distXZ = Vector2.Distance(transform.position.ConvertVector3To2(1), navMeshAgent.destination.ConvertVector3To2(1));
            return distXZ < alomstArrived;
        }
    }
    Coroutine CheckingUntilArrive;
    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        navMeshObstacle.enabled = false;
    }
    public void StartJob(ModelAnimationPlayerJobManager.ModelHandlerJob job)
    {
        if (job.ap != null)
        {
            if (CheckingUntilArrive != null)
                StopCoroutine(CheckingUntilArrive);

            TurnOnNavi(true);
            if (ShouldNavControllerWorking(job.ap, out Vector3 correctVector))
            {
                CheckingUntilArrive = StartCoroutine(DoCheckUntilArrive(job, correctVector));
            }
            else
            {
                TurnOnNavi(false);
                job.EndJob();
            }
        }
    }
    IEnumerator DoCheckUntilArrive(ModelAnimationPlayerJobManager.ModelHandlerJob job, Vector3 correctedVector)
    {
        navMeshAgent.isStopped = false;
        var shouldAlertTraffic = false;
        var ap = job.ap;
        var lastPosition = ap.transform.position;

        while (!IsArrivedDestination)
        {
            if (lastPosition != ap.transform.position)
            {
                SetDestination(ap.transform.position, out correctedVector);
                lastPosition = ap.transform.position;
            }

            if (!shouldAlertTraffic
                    && IsAlmostArrivedDestination)
            {
                shouldAlertTraffic = true;
                var colliders = Physics.OverlapSphere(correctedVector, alomstArrived * 1.5f).ToList();
                var find = colliders.Find(x =>
                {
                    var agent = x.GetComponent<NavMeshAgent>();
                    return agent != null && agent != navMeshAgent;
                });

                if (find != null)
                {
                    var canYield = true;
                    if (ap.CanYield)
                        canYield = NaviTrafficManager.Instance.AddCasePoint(correctedVector, this, job.ap);

                    if (!canYield)
                    {
                        var shouldGoLeft = Random.Range(0, 2) == 1;
                        var point = correctedVector + Quaternion.Euler(0f, 90f * (shouldGoLeft ? -1 : 1), 0f) * transform.forward * 2f;
                        Debug.Log(point + "//" + correctedVector);
                        SetDestination(point, out correctedVector);

                        if (ap.ShouldPlaySamePosition)
                        {
                            ap.ReplaceExpectionState();
                            Debug.Log("Targeting place is very Busy now. Will Playing Expaction AniState // Targeting Position : " + correctedVector);
                        }

                        GizmosDrawer.instanse.DrawChangePoint(transform.position, correctedVector, 2f);
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }

        TurnOnNavi(false);

        ap.CorrectedPosition = correctedVector;
        job.EndJob();
    }

    bool ShouldNavControllerWorking(AnimationPoint ap, out Vector3 correctiondVector)
    {
        SetDestination(ap.transform.position, out correctiondVector);
        return !IsArrivedDestination;
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

    void SetDestination(Vector3 targetPosition, out Vector3 correctiondVector)
    {
        correctiondVector = Vector3.zero;
        if (IsPositionCanReach(targetPosition, out NavMeshHit hit))
        {
            correctiondVector = hit.position;
            navMeshAgent.SetDestination(correctiondVector);
            GizmosDrawer.instanse.DrawLine(targetPosition, correctiondVector, 2f, Color.magenta);
            GizmosDrawer.instanse.DrawSphere(correctiondVector, 0.05f, 2f, Color.magenta);
        }
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