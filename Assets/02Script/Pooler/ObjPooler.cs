using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ObjPooler : MonoBehaviour
{
    public GameObject TargetObj;
    protected Queue<GameObject> instantiatedObj = new Queue<GameObject>();
    public int ObjCount { get => instantiatedObj.Count; }
    public int makingCountInOneTime = 2;
    public int minimumMaintenanceCost = 1;
    private int pooledCount = 0;
    public float Availability
    {
        get
        {
            var pooledCountCopy = pooledCount;
            pooledCount = 0;
            return math.lerp(0, ObjCount, pooledCountCopy);
        }
    }

    private object makingNewOneSection = new object();
    private void Start()
    {
        if (TargetObj.GetComponent<IPoolerConnector>() == null)
        {
            Debug.Log("Target obj must has IPoolerConnector for reset when get");
        }
    }
    public bool IsTarget<T>() => TargetObj.GetComponent<T>() != null;
    public void MakeNewOne()
    {
        for (int i = 0; i < makingCountInOneTime; i++)
            instantiatedObj.Enqueue(Instantiate(TargetObj, transform).gameObject);
    }
    public void MakeNewOne(int count)
    {
        Queue<Action> actions = new Queue<Action>();
        for (int i = 0; i < count; i++)
            actions.Enqueue(() => { MakeNewOne(); });

        DistributedProcessingManager.Instance.AddJob(makingNewOneSection, actions);
    }

    public void ManagePoolerCapacity(float rate)
    {
        var targetCount = Mathf.FloorToInt(Mathf.InverseLerp(0, ObjCount, rate));
        targetCount = Mathf.Max(targetCount, minimumMaintenanceCost);
        for (int i = 0; i < targetCount; i++)
        {
            var target = GetNewOne();
            Destroy(target);
        }
    }

    public bool CanPull(int i) { return instantiatedObj.Count >= i; }
    public bool CanPull() => CanPull(1);

    public GameObject GetNewOne()
    {
        if (!CanPull())
            MakeNewOne();

        var obj = instantiatedObj.Dequeue();
        obj.GetComponent<IPoolerConnector>().ResetObj();
        return obj;
    }

    public T GetNewOne<T>()
    {
        return GetNewOne().GetComponent<T>();
    }

    public void ReturnTargetObj(GameObject TargetObj)
    {
        TargetObj.SetActive(false);
        TargetObj.transform.SetParent(transform);
        instantiatedObj.Enqueue(TargetObj);
    }

    public static void CopyComponentValue(Component from, Component to)
    {
        if (from is Transform)
        {
            var fTransform = from as Transform;
            var tTransform = to as Transform;

            tTransform.position = fTransform.position;
            tTransform.rotation = fTransform.rotation;
            tTransform.localScale = fTransform.localScale;
        }
        else
        {
            foreach (var field in from.GetType().GetFields())
            {
                field.SetValue(to, field.GetValue(from));
            }
        }
    }
}
