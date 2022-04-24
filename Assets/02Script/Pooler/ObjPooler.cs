using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPooler : MonoBehaviour
{
    public GameObject TargetObj;
    protected Queue<GameObject> instantiatedObj = new Queue<GameObject>();
    public static void CopyComponentValue(Component from, Component to)
    {
        foreach (var field in to.GetType().GetFields())
        {
            field.SetValue(from, field.GetValue(to));
        }
    }

    public void MakeNewOne()
    {
        instantiatedObj.Enqueue(Instantiate(TargetObj, transform).gameObject);
    }

    public bool CanPull(int i) { return instantiatedObj.Count >= i; }
    public bool CanPull() => CanPull(0);

    public GameObject GetNewOne()
    {
        if (!CanPull())
            MakeNewOne();

        return instantiatedObj.Dequeue();
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
}
