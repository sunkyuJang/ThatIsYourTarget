using System.Collections;
using System.Collections.Generic;
using Autohand;
using UnityEngine;
public class RagDollHandler : MonoBehaviour
{
    Rigidbody[] rigidbodies;
    Collider[] colliders;
    void Awake()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();

        TrunOnRigid(false);
    }

    public void TrunOnRigid(bool shouldTurnOn)
    {
        foreach (var item in rigidbodies)
        {
            item.isKinematic = !shouldTurnOn;
            if (shouldTurnOn)
            {
                var grabbable = item.gameObject.AddComponent<Grabbable>();
                grabbable.parentOnGrab = false;
            }
        }
    }
}
