using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPooler : MonoBehaviour
{
    public GameObject TargetObj;
    Queue<GameObject> instantiatedObj = new Queue<GameObject>();

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
