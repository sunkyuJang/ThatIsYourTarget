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

    public enum AliveState { Alive, Stun, Dead }
    public AliveState NowAliveState { protected set; get; } = AliveState.Alive;

    public enum AlertLevel { Normal, Notice, Attack, Avoid }
    public AlertLevel NowAlertLevel { protected set; get; } = AlertLevel.Normal;

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
        model.SetNextPosition(NextPosition.position);
    }

    public void GetHit()
    {

        model.GetHit();
    }

    Transform GetNextPosition()
    {
        if (Vector3.Distance(model.transform.position, NextPosition.position) < 1f)
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

    public void SetBelongTo(Material material)
    {
        model.SetBelong(material);
    }
}
