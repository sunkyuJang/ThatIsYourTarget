using System.Collections.Generic;
using UnityEngine;

public class ObjPooler : MonoBehaviour
{
    public GameObject TargetObj;
    protected Queue<GameObject> instantiatedObj = new Queue<GameObject>();
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

    public bool IsTarget<T>() { return TargetObj.GetComponent<T>() != null; }

    public void MakeNewOne()
    {
        instantiatedObj.Enqueue(Instantiate(TargetObj, transform).gameObject);
    }

    public bool CanPull(int i) { return instantiatedObj.Count >= i; }
    public bool CanPull() => CanPull(1);

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
