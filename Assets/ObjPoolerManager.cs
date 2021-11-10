using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPoolerManager : MonoBehaviour
{
    public static ObjPoolerManager Instance { set; get; }
    List<ObjPooler> objPoolers = new List<ObjPooler>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public ObjPooler GetPooler(GameObject TargetObj)
    {
        ObjPooler nowPooler = null;
        for (int i = 0; i < objPoolers.Count; i++)
        {
            if (objPoolers[i].TargetObj.Equals(TargetObj))
            {
                nowPooler = objPoolers[i];
                break;
            }
        }

        if (nowPooler == null)
        {
            nowPooler = GetNewPooler(TargetObj);
        }

        return nowPooler;
    }

    ObjPooler GetNewPooler(GameObject TargetObj)
    {
        var newPooler = new GameObject(TargetObj.name).AddComponent<ObjPooler>();
        newPooler.transform.SetParent(transform);
        newPooler.TargetObj = TargetObj;

        return newPooler;
    }
}
