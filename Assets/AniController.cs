using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RagDollHandler))]
public class AniController : MonoBehaviour
{
    RagDollHandler ragDollHandler { set; get; }
    public ModelPhysicsController modelPhysicsController;
    public Animator animator;
    private void Awake()
    {
        ragDollHandler = GetComponent<RagDollHandler>();
        animator = GetComponent<Animator>();
    }

    protected void FixedUpdate()
    {
        if (modelPhysicsController.naviController.navMeshAgent != null)
        {
            var NavMeshAgent = modelPhysicsController.naviController.navMeshAgent;

            var degree = Mathf.Round(Vector3.Angle(transform.forward, NavMeshAgent.velocity.normalized));
            var cross = Vector3.Cross(transform.forward, NavMeshAgent.velocity.normalized);
            degree *= cross.y >= 0 ? 1 : -1;

            animator.SetFloat("WalkY", Mathf.Cos(degree * Mathf.Deg2Rad));
            animator.SetFloat("WalkX", Mathf.Sin(degree * Mathf.Deg2Rad));
        }
    }

    public virtual void StartAni(ActionPoint actionPoint) { }
}
