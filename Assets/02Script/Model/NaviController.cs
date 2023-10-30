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

            CheckingUntilArrive = StartCoroutine(DoCheckUntilArrive(job));
        }
    }
    IEnumerator DoCheckUntilArrive(ModelAnimationPlayerJobManager.ModelHandlerJob job)
    {
        var ap = job.ap;
        TurnOnNavi(true);
        var lastPosition = ap.transform.position;
        SetDestination(ap.transform.position, out Vector3 correctiondVector);
        while (!IsArrivedDestination)
        {
            if (lastPosition != ap.transform.position)
            {
                SetDestination(ap.transform.position, out correctiondVector);
                lastPosition = ap.transform.position;
            }
            yield return new WaitForFixedUpdate();
        }

        TurnOnNavi(false);

        ap.transform.position = correctiondVector;
        job.EndJob();
    }

    Vector3 CorrectPosition(Vector3 position, Vector3 dir)
    {
        var collideRadius = navMeshAgent.radius * 1.2f;
        var maxIterations = 5;
        var hitPoint = transform.GetSurroundingCastHitPosition(45f, collideRadius);
        if (hitPoint.Any())
        {
            hitPoint.ForEach(x =>
            {

            });
        }
        for (int interation = 0; interation < maxIterations; interation++)
        {
            if (!Physics.Raycast(position, dir, out RaycastHit hit, collideRadius))
            {
                break;
            }
            else
            {
                if (Mathf.Abs(hit.normal.y) < 0.1f)
                {
                    Vector3 offsetPosition = hit.point - (dir.normalized * collideRadius);
                    var tempPosition = new Vector3(offsetPosition.x, position.y, offsetPosition.z);
                    if (tempPosition == position)
                        interation = maxIterations;

                    position = tempPosition;
                }
                else
                {
                    break;
                }
            }
        }
        return position;
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