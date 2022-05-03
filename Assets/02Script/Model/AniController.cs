using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniController : MonoBehaviour
{
    protected RagDollHandler ragDollHandler { set; get; }
    protected ModelPhysicsController modelPhysicsController;
    protected Animator animator;
    protected void Awake()
    {
        ragDollHandler = GetComponent<RagDollHandler>();
        modelPhysicsController = GetComponent<ModelPhysicsController>();
        animator = GetComponent<Animator>();
    }

    protected void Start()
    {
        StartCoroutine(DoWalking());
    }

    IEnumerator DoWalking()
    {
        while (true)
        {
            if (modelPhysicsController.naviController.navMeshAgent != null)
            {
                var NavMeshAgent = modelPhysicsController.naviController.navMeshAgent;
                if (modelPhysicsController.naviController.IsPositionAndRotationGetCorrect)
                {
                    animator.SetFloat("WalkY", 0);
                    animator.SetFloat("WalkX", 0);
                }
                else
                {
                    var degree = Mathf.Round(Vector3.Angle(transform.forward, NavMeshAgent.velocity.normalized));
                    var cross = Vector3.Cross(transform.forward, NavMeshAgent.velocity.normalized);
                    degree *= cross.y >= 0 ? 1 : -1;

                    animator.SetFloat("WalkY", Mathf.Cos(degree * Mathf.Deg2Rad));
                    animator.SetFloat("WalkX", Mathf.Sin(degree * Mathf.Deg2Rad));
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
    public virtual void StartAni(ActionPoint actionPoint) { }
}
