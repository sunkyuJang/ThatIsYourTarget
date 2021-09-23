using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PersonModel : MonoBehaviour
{
    Animator animator;
    Rigidbody Rigidbody { set; get; } = null;
    RagDollHandler ragDollHandler;
    NavMeshAgent NavMeshAgent { set; get; }
    public Renderer ModelRender { protected set; get; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        ragDollHandler = GetComponent<RagDollHandler>();
        NavMeshAgent = GetComponent<NavMeshAgent>();

        for (int i = 0; i < transform.childCount; i++)
        {
            var nowObj = transform.GetChild(i).gameObject;
            if (nowObj.activeSelf)
            {
                ModelRender = nowObj.GetComponent<Renderer>();
                break;
            }
        }
    }

    public void SetNextPosition(Vector3 worldPosition)
    {
        NavMeshAgent.SetDestination(worldPosition);
    }

    private void FixedUpdate()
    {
        var degree = Mathf.Round(Vector3.Angle(transform.forward, NavMeshAgent.velocity.normalized));
        var cross = Vector3.Cross(transform.forward, NavMeshAgent.velocity.normalized);
        degree *= cross.y >= 0 ? 1 : -1;

        animator.SetFloat("WalkY", Mathf.Cos(degree * Mathf.Deg2Rad));
        animator.SetFloat("WalkX", Mathf.Sin(degree * Mathf.Deg2Rad));
    }

    public virtual void GetHit()
    {
        animator.enabled = false;
        ragDollHandler.TrunOnRigid(true);
        NavMeshAgent.enabled = false;
    }

    public void SetBelong(Material material)
    {
        ModelRender.material = material;
    }

    public void MakeLookAt(Vector3 dir)
    {
        NavMeshAgent.isStopped = true;
        StartCoroutine(DoLookAtWithSpeed(dir));
    }
    IEnumerator DoLookAtWithSpeed(Vector3 dir)
    {
        var startForward = transform.forward;
        var oppositForward = transform.forward * -1f;
        var t = 0f;
        var maxT = 2f;
        var xRatio = 1 - Mathf.InverseLerp(transform.forward.x, oppositForward.x, dir.x);
        t = Mathf.Lerp(0, maxT, xRatio);

        while (t < maxT)
        {
            var nowRatio = Mathf.Lerp(0, maxT, t);
            transform.forward = Vector3.Lerp(startForward, dir, nowRatio);
            t += Time.fixedDeltaTime;

            //if (Vector3.Distance(transform.forward, dir) < 0.05f) break;

            yield return new WaitForFixedUpdate();
        }
        print(true);
    }
    // IEnumerator DoLookAtWithSpeed(Vector3 dir)
    // {
    //     var dirWithQ = Quaternion.Euler(dir.x, dir.y, dir.z);
    //     var nowRotation = transform.forward;
    //     while (true)
    //     {
    //         transform.forward = Vector3.Slerp(nowRotation, dir, NavMeshAgent.speed);

    //         yield return new WaitForFixedUpdate();

    //         if (Vector3.Distance(transform.forward, dir) < 0.05f) break;
    //     }
    // }

    public void SetSittingAnimation(int sittingLevel)
    {
        animator.SetInteger("SittingLevel", sittingLevel);
        NavMeshAgent.isStopped = true;
    }

    public void SetToIdleAnimation()
    {
        animator.SetInteger("SittingLevel", 0);
        animator.SetBool("LookAround", false);
        animator.SetBool("ShouldTurn", false);
        StartCoroutine(DoNavMeshAgentWork());
    }

    IEnumerator DoNavMeshAgentWork()
    {
        yield return new WaitForSeconds(2f);

        NavMeshAgent.isStopped = false;
    }
}
