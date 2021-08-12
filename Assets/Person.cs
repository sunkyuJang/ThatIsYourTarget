using System.Collections;
using System.Collections.Generic;
using SensorToolkit;
using UnityEngine;
using UnityEngine.AI;


public class Person : MonoBehaviour, IObjDetectorConnector_OnContecting
{
    PersonModel model;

    Transform TrackingPositionGroup;
    Transform NextPosition { set; get; } = null;

    enum AliveState { Alive, Stun, Dead }
    AliveState nowAliveState = AliveState.Alive;

    enum AlertLevel { Normal, Notice, Attack, Avoid }
    AlertLevel nowAlertLevel = AlertLevel.Normal;


    private void Awake()
    {
        model = transform.Find("Model").GetComponent<PersonModel>();

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

    public void OnContecting(ObjDetector detector, Collider collider)
    {

        //throw new System.NotImplementedException();
    }
}
