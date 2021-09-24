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
        var cross = Vector3.Cross(Vector3.up, startForward);
        var dot = Vector3.Dot(cross, dir);
        var isLeft = dot < 0;

        var lastAngle = Vector3.Angle(transform.forward, dir);
        while (true)
        {
            transform.Rotate(isLeft ? Vector3.down : Vector3.up, 200f * Time.fixedDeltaTime);
            var nowAngle = Vector3.Angle(transform.forward, dir);
            if (nowAngle > lastAngle) break;
            else lastAngle = nowAngle;
            yield return new WaitForFixedUpdate();
        }
        yield return null;

    }

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
