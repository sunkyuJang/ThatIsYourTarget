using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjDetector : MonoBehaviour
{
    public GameObject connectedObj;
    public List<string> targetTags = new List<string>();
    public bool shouldFindByName = false;

    protected Collider Collider { set; get; }
    protected IObjDetectorConnector_OnDetected I_OnDetected { set; get; }
    protected IObjDetectorConnector_OnContecting I_OnContecting { set; get; }
    protected IObjDetectorConnector_OnRemoved I_OnRemoved { set; get; }

    private void Start()
    {
        //check collider
        Collider = gameObject.GetComponent<Collider>();
        if (Collider == null)
            Debug.Log("collider not Attached");
        else
            Collider.isTrigger = true;

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
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (IsFind(other))
        {
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
}