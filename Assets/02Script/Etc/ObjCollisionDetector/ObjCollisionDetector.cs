using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjCollisionDetector : MonoBehaviour
{
    public GameObject connectedObj;
    public List<string> targetTags = new List<string>();
    public bool shouldFindByName = false;
    public bool isAllowEverthing = false;

    protected Collider DectectCollider { set; get; }
    protected IObjCollisionDetectorConnector_OnCollisionEnter I_CollisionEnter { set; get; }


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
            I_CollisionEnter = connectedObj.GetComponent<IObjCollisionDetectorConnector_OnCollisionEnter>();
        }
    }
    private void Start()
    {
        SetDetectCollider();
        SetInterface();
    }

    protected virtual private void OnCollisionEnter(Collision other)
    {
        if (IsFind(other.collider))
        {
            I_CollisionEnter.OnCollisionEnterByConnector(this, other);
        }
    }

    protected bool IsFind(Collider other)
    {
        if (isAllowEverthing)
        {
            return true;
        }
        else
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
}
