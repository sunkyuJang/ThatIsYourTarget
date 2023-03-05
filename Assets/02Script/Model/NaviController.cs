using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NavMeshObstacle))]
public class NaviController : MonoBehaviour, IJobStarter
{
    ModelHandler modelPhysicsController;
    public NavMeshAgent navMeshAgent { private set; get; }
    NavMeshObstacle navMeshObstacle { set; get; }
    public float permissibleRangeToDestination = 0.01f;
    public bool IsArrivedDestination { get { return Vector3.Distance(transform.position, navMeshAgent.destination) < permissibleRangeToDestination; } }
    public Coroutine CheckingUntilArrive;
    void Awake()
    {
        modelPhysicsController = GetComponent<ModelHandler>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        navMeshObstacle.enabled = false;
    }
    public void StartJob(Job jobOption)
    {
        if (jobOption is ModelHandler.ModelJob)
        {
            var job = jobOption as ModelHandler.ModelJob;
            if (job.ap != null)
            {
                var ap = job.ap;
                TurnOnNavi(true);
                navMeshAgent.SetDestination(ap.transform.position);

                if (CheckingUntilArrive != null)
                {
                    StopCoroutine(CheckingUntilArrive);
                }

                CheckingUntilArrive = StartCoroutine(DoCheckUntilArrive(job));
            }
        }
    }
    IEnumerator DoCheckUntilArrive(ModelHandler.ModelJob job)
    {
        var ap = job.ap;
        TurnOnNavi(true);
        var lastPosition = ap.transform.position;

        while (!IsArrivedDestination)
        {
            if (lastPosition != ap.transform.position)
            {
                navMeshAgent.SetDestination(ap.transform.position);
                lastPosition = ap.transform.position;
            }
            yield return new WaitForFixedUpdate();
        }

        TurnOnNavi(false);

        job.EndJob();
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
