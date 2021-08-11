using System.Collections;
using System.Collections.Generic;
using SensorToolkit;
using UnityEngine;
using UnityEngine.AI;


public class Person : MonoBehaviour
{
    Transform Model { set; get; }
    Animator animator;
    Rigidbody Rigidbody { set; get; } = null;
    RagDollHandler ragDollHandler;
    NavMeshAgent NavMeshAgent { set; get; }


    Transform TrackingPositionGroup;
    Transform NextPosition { set; get; } = null;
    public FOVCollider FOVCollider;

    private void Awake()
    {
        Model = transform.Find("Model");
        animator = Model.GetComponent<Animator>();
        Rigidbody = Model.GetComponent<Rigidbody>();
        ragDollHandler = Model.GetComponent<RagDollHandler>();
        NavMeshAgent = Model.GetComponent<NavMeshAgent>();

        TrackingPositionGroup = transform.Find("TrackingPositionGroup");
        NextPosition = TrackingPositionGroup.GetChild(0);
    }

    private void FixedUpdate()
    {
        var nextPosition = GetNextPosition();
        NextPosition = nextPosition == null ? NextPosition : nextPosition;
        NavMeshAgent.SetDestination(NextPosition.position);
        var degree = Mathf.Round(Vector3.Angle(Model.forward, NavMeshAgent.velocity.normalized));
        var cross = Vector3.Cross(Model.forward, NavMeshAgent.velocity.normalized);
        degree *= cross.y >= 0 ? 1 : -1;
        // print(degree);
        // print("y =" + Mathf.Sin(degree * Mathf.Deg2Rad));
        // print("x =" + Mathf.Cos(degree * Mathf.Deg2Rad));

        animator.SetFloat("WalkY", Mathf.Cos(degree * Mathf.Deg2Rad));
        animator.SetFloat("WalkX", Mathf.Sin(degree * Mathf.Deg2Rad));

    }
    public virtual void GetHit()
    {
        animator.enabled = false;
        ragDollHandler.TrunOnRigid(true);
        NavMeshAgent.enabled = false;
    }

    Transform GetNextPosition()
    {
        if (Vector3.Distance(Model.position, NextPosition.position) < 1f)
        {
            var index = NextPosition.GetSiblingIndex();
            index = (index + 1) % TrackingPositionGroup.childCount;

            return TrackingPositionGroup.GetChild(index);
        }
        return null;
    }

}
