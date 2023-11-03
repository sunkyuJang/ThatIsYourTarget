using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using JExtentioner;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NavMeshObstacle))]
public class NaviController : MonoBehaviour, IJobStarter<ModelAnimationPlayerJobManager.ModelHandlerJob>
{
    NavMeshAgent navMeshAgent { set; get; }
    NavMeshObstacle navMeshObstacle { set; get; }
    float permissibleRangeToDestinationXZ = 0.01f;
    bool IsArrivedDestination
    {
        get
        {
            var distXZ = Vector2.Distance(transform.position.ConvertVector3To2(1), navMeshAgent.destination.ConvertVector3To2(1));
            return distXZ < permissibleRangeToDestinationXZ;
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
            if (ShouldNavControllerWorking(job.ap))
            {
                CheckingUntilArrive = StartCoroutine(DoCheckUntilArrive(job));
            }
            else
            {
                TurnOnNavi(false);
                job.EndJob();
            }
        }
    }
    IEnumerator DoCheckUntilArrive(ModelAnimationPlayerJobManager.ModelHandlerJob job)
    {
        var ap = job.ap;
        var lastPosition = ap.transform.position;
        var correctedVector = lastPosition;
        while (!IsArrivedDestination)
        {
            if (lastPosition != ap.transform.position)
            {
                SetDestination(ap.transform.position, out correctedVector);
                lastPosition = ap.transform.position;
            }
            yield return new WaitForFixedUpdate();
        }

        TurnOnNavi(false);

        ap.transform.position = correctedVector;
        job.EndJob();
    }

    bool ShouldNavControllerWorking(AnimationPoint ap)
    {
        SetDestination(ap.transform.position, out Vector3 correctiondVector);
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