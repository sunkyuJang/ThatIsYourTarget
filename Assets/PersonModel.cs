using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PersonModel : MonoBehaviour
{
    Transform Model { set; get; }
    Animator animator;
    Rigidbody Rigidbody { set; get; } = null;
    RagDollHandler ragDollHandler;
    NavMeshAgent NavMeshAgent { set; get; }

    private void Awake()
    {
        Model = transform.Find("Model");
        animator = Model.GetComponent<Animator>();
        Rigidbody = Model.GetComponent<Rigidbody>();
        ragDollHandler = Model.GetComponent<RagDollHandler>();
        NavMeshAgent = Model.GetComponent<NavMeshAgent>();
    }
}
