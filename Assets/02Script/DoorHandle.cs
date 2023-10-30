using System.Collections;
using System.Collections.Generic;
using Autohand;
using UnityEngine;

public class DoorHandle : MonoBehaviour//, IObjDetectorConnector_OnRemoved
{
    // Grabbable grabbable;
    // Hand GrabbingHand { set; get; }
    // private void Awake()
    // {
    //     grabbable = GetComponent<Grabbable>();
    // }
    // public void OnHolding()
    // {
    //     var heldHands = grabbable.GetHeldBy();
    //     foreach (var hand in heldHands)
    //     {
    //         if (hand.IsGrabbing())
    //         {
    //             GrabbingHand = hand;
    //         }
    //     }
    // }
    // public void OnRemoved(ObjDetector detector, Collider collider)
    // {
    //     print(collider.gameObject.name);
    //     print(GrabbingHand.GetMoveTo.gameObject.name);

    //     if (collider.transform.Equals(GrabbingHand.GetMoveTo))
    //     {
    //         print(true);
    //         grabbable.ForceHandsRelease();
    //     }
    // }
}
