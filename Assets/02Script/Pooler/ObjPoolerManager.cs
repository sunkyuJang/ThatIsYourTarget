using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPoolerManager : MonoBehaviour
{
    public static ObjPoolerManager Instance { set; get; }
    List<ObjPooler> objPoolers = new List<ObjPooler>();

    [Header("DistributedProcessingManager memory handle")]
    object sectionObj = new object();
    [SerializeField] float managingPoolerTimeUnit = 10f;
    [SerializeField] float minAvailability = 0.5f;
    [SerializeField] float decreasePoolerCapacity = 0.75f;
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

    IEnumerator ManagingPooler()
    {
        yield return new WaitForSeconds(managingPoolerTimeUnit);
        Queue<Action> actions = new Queue<Action>();
        objPoolers.ForEach(pooler =>
        {
            Action action = () =>
            {
                bool substandard = pooler.Availability < minAvailability;
                pooler.ManagePoolerCapacity(decreasePoolerCapacity);
            };

            actions.Enqueue(action);
        });

        actions.Enqueue(() => StartCoroutine(ManagingPooler()));
        DistributedProcessingManager.Instance.AddJob(sectionObj, actions);
    }
}
