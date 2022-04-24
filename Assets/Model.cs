using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    int state = 0;
    ModelPhysicsController modelPhysicsController;
    public ActionPointHandler originalAPH;

    private void Awake()
    {
        modelPhysicsController = transform.Find("modelPhysicsController").GetComponent<ModelPhysicsController>();
    }
    private void Start()
    {

    }

    public void ContectingObj(Collider target)
    {
        // if (target.tag.CompareTo("Player"))
        // {

        // }
    }
}
