using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractionObjGrabRigHandler : MonoBehaviour
{
    public List<InteractionObjGrabRig> grabRigs = new List<InteractionObjGrabRig>();

    public void SetGrabRigs()
    {
        grabRigs.Clear();
        grabRigs = GetComponentsInChildren<InteractionObjGrabRig>().ToList();
    }

    public bool Controll_IK(GameObject gameObject, bool turnOn)
    {
        var rig = grabRigs.Find(x => x.IsSamePrefab(gameObject));
        if (rig != null)
        {
            rig.TurnOn_IK(turnOn);
            return true;
        }

        return false;
    }
}
