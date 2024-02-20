using Autohand;
using UnityEngine;
public class RagDollHandler
{
    Rigidbody[] Rigidbodies { set; get; }
    Rigidbody ActorRigid { set; get; }
    Collider ActorCollider { set; get; }
    public RagDollHandler(Transform actor)
    {
        Rigidbodies = actor.GetComponentsInChildren<Rigidbody>();
        ActorRigid = actor.GetComponent<Rigidbody>();
        ActorCollider = actor.GetComponent<Collider>();
        BeRagDollState(false);
    }

    public void BeRagDollState(bool shouldTurnOn)
    {
        ActorRigid.isKinematic = shouldTurnOn;
        //ActorCollider.enabled = !shouldTurnOn;

        foreach (var item in Rigidbodies)
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