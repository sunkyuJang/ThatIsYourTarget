using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using JMath;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NavMeshObstacle))]
public class NavController : MonoBehaviour
{
    ModelPhysicsController modelPhysicsController;
    public NavMeshAgent navMeshAgent { private set; get; }
    NavMeshObstacle navMeshObstacle { set; get; }
    public float permissibleRangeToDestination = 0.01f;
    public bool IsArrivedDestination { get { return Vector3.Distance(transform.position, navMeshAgent.destination) < permissibleRangeToDestination; } }
    public Coroutine CheckingUntilArrive;
    void Awake()
    {
        modelPhysicsController = GetComponent<ModelPhysicsController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        navMeshObstacle.enabled = false;
    }
    public void StopAllProcess() { StopAllCoroutines(); }
    public void SetNextPosition(ActionPoint ap)
    {
        TurnOnNavi(true);
        navMeshAgent.SetDestination(ap.transform.position);

        if (CheckingUntilArrive != null)
        {
            StopCoroutine(CheckingUntilArrive);
        }

        CheckingUntilArrive = StartCoroutine(DoCheckUntilArrive(ap));
    }

    IEnumerator DoCheckUntilArrive(ActionPoint ap)
    {
        yield return new WaitUntil(() => IsArrivedDestination);
        TurnOnNavi(false);
        modelPhysicsController.ReadNowAction();
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
}
