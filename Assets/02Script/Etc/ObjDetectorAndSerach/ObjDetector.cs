using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjDetector : MonoBehaviour
{
    public GameObject connectedObj;
    public List<string> targetTags = new List<string>();
    public bool shouldFindByName = false;

    protected Collider DectectCollider { set; get; }
    protected IObjDetectorConnector_OnDetected I_OnDetected { set; get; }
    protected IObjDetectorConnector_OnContecting I_OnContecting { set; get; }
    protected IObjDetectorConnector_OnRemoved I_OnRemoved { set; get; }

    public enum DrawTarget { OnlyClosedOne, AllTheTarget, Non }
    public DrawTarget drawTarget = DrawTarget.OnlyClosedOne;
    public enum LineKind { Collide, Pass }
    public LineKind lineKind = LineKind.Collide;
    List<Transform> Targets { set; get; } = new List<Transform>();
    Transform Parent { set; get; }
    protected virtual void SetDetectCollider()
    {
        //check collider
        DectectCollider = gameObject.GetComponent<Collider>();
        if (DectectCollider == null)
            Debug.Log("collider not Attached");
        else
            DectectCollider.isTrigger = true;
    }

    protected virtual void SetInterface()
    {
        //check Connected Obj
        if (connectedObj == null)
            Debug.Log("no Connected Obj");
        else
        {
            I_OnDetected = connectedObj.GetComponent<IObjDetectorConnector_OnDetected>();
            I_OnContecting = connectedObj.GetComponent<IObjDetectorConnector_OnContecting>();
            I_OnRemoved = connectedObj.GetComponent<IObjDetectorConnector_OnRemoved>();
        }
    }
    private void Start()
    {
        SetDetectCollider();
        SetInterface();
        DrawLine();
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (IsFind(other))
        {
            SetTarget(other, true);
            I_OnDetected?.OnDetected(this, other);
        }
    }
    protected virtual void OnTriggerStay(Collider other)
    {
        if (IsFind(other))
        {
            I_OnContecting?.OnContecting(this, other);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (IsFind(other))
        {
            SetTarget(other, false);
            I_OnRemoved?.OnRemoved(this, other);
        }
    }

    protected bool IsFind(Collider other)
    {
        var nowString = shouldFindByName ? other.gameObject.name : other.tag;
        foreach (var item in targetTags)
        {
            if (item.Equals(nowString))
                return true;
        }

        return false;
    }

    protected void SetTarget(Collider collider, bool isAdding)
    {
        if (drawTarget != DrawTarget.Non)
        {
            if (IsFind(collider))
            {
                var resistedTarget = Targets.Find(x => x == collider.transform);

                if (isAdding)
                {
                    if (resistedTarget == null)
                    {
                        for (int i = 0; i <= Targets.Count; i++)
                        {
                            if (i == Targets.Count)
                            {
                                Targets.Add(collider.transform);
                                break;
                            }

                            var nowTarget = Targets[i];
                            var nowTargetDist = Vector3.Distance(transform.position, nowTarget.position);
                            var targetDist = Vector3.Distance(transform.position, collider.transform.position);

                            if (targetDist < nowTargetDist)
                            {
                                Targets.Insert(i, collider.transform);
                            }
                        }
                    }
                }
                else
                {
                    if (resistedTarget != null)
                        Targets.Remove(collider.transform);
                }
            }
        }
    }

    protected void DrawLine()
    {
        StartCoroutine(DoDrawLine());
    }
    protected IEnumerator DoDrawLine()
    {
        while (true)
        {
            if (drawTarget != DrawTarget.Non)
            {
                for (int i = 0; i < Targets.Count; i++)
                {
                    var target = Targets[i];
                    var dir = target.position - transform.position;
                    var dist = Vector3.Distance(transform.position, target.position);
                    var startPoint = transform.position;
                    for (bool isExceptSelf = false; isExceptSelf == false;)
                    {
                        Physics.Raycast(startPoint, dir, out RaycastHit hit, dist);
                        if (hit.transform != transform && !hit.transform.IsChildOf(transform.root))
                        {
                            Debug.DrawLine(transform.position, hit.point, hit.transform == target ? Color.green : Color.red, 2f);
                            isExceptSelf = true;
                        }
                        else
                        {
                            startPoint = hit.point + dir * 0.01f;
                            dist = Vector3.Distance(startPoint, target.position);
                        }
                    }

                    if (drawTarget == DrawTarget.OnlyClosedOne)
                        break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
