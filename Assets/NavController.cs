using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    bool isPositionCorrect = false;
    bool isRotationCorrect = false;
    public bool IsPositionAndRotationGetCorrect { get { return isPositionCorrect && isRotationCorrect; } }
    void Awake()
    {
        modelPhysicsController = GetComponent<ModelPhysicsController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        navMeshObstacle.enabled = false;
    }

    public void SetNextPosition(Vector3 WPosition)
    {
        TurnOnNavi(true);
        isPositionCorrect = false;
        isRotationCorrect = false;
        navMeshAgent.SetDestination(WPosition);
        CheckingUntilArrive = StartCoroutine(DoCheckUntilArrive());
    }

    IEnumerator DoCheckUntilArrive()
    {
        yield return new WaitUntil(() => IsArrivedDestination);
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

    public void MakeCorrect(Vector3 WPosition, Vector3 forward)
    {
        navMeshAgent.isStopped = true;
        TurnOnNavi(false);
        SetPositionCorrectly(WPosition);
        MakeLookAt(new Vector3(forward.x, 0f, forward.z));
    }

    public void SetPositionCorrectly(Vector3 worldPosition)
    {
        StartCoroutine(DoSetPositionCorrectly(worldPosition));
    }

    IEnumerator DoSetPositionCorrectly(Vector3 worldPosition)
    {
        var t = 0f;
        var maxT = 0.5f;
        var beforePosition = transform.position;
        while (t < maxT)
        {
            yield return new WaitForFixedUpdate();
            t += Time.fixedDeltaTime;
            var ratio = Mathf.InverseLerp(0, maxT, t);
            transform.position = Vector3.Lerp(beforePosition, worldPosition, ratio);
        }
        isPositionCorrect = true;
    }

    public void MakeLookAt(Vector3 dir)
    {
        StartCoroutine(DoLookAtWithSpeed(dir));
    }
    IEnumerator DoLookAtWithSpeed(Vector3 dir)
    {
        //Roughly
        var startForward = transform.forward;
        var cross = Vector3.Cross(Vector3.up, startForward);
        var dot = Vector3.Dot(cross, dir);
        var isLeft = dot < 0;
        var rotateSpeed = 300f;
        var lastAngle = Vector3.Angle(transform.forward, dir);
        while (true)
        {
            transform.Rotate(isLeft ? Vector3.down : Vector3.up, rotateSpeed * Time.fixedDeltaTime);
            var nowAngle = Vector3.Angle(transform.forward, dir);
            if (nowAngle > lastAngle) break;
            else lastAngle = nowAngle;
            yield return new WaitForFixedUpdate();
        }

        //Correctly
        if (Vector3.Angle(transform.forward, dir) * Mathf.Rad2Deg > 3f)
        {
            var t = 0f;
            var maxT = 1f;
            startForward = transform.forward;
            while (t < maxT)
            {
                var ratio = Mathf.InverseLerp(0, maxT, t);
                transform.forward = Vector3.Lerp(startForward, dir, ratio);
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        isRotationCorrect = true;
        yield return null;
    }
}
