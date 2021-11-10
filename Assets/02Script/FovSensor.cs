using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FovSensor : ObjDetector
{
    public Vector3 rayStartPoint { get { return transform.position + transform.forward * 0.2f; } }
    protected override void OnTriggerStay(Collider other)
    {
        if (IsFind(other))
        {
            if (IsTargetOnlyCloseOne(other))
            {
                Debug.DrawLine(rayStartPoint, other.transform.position, Color.red, 2f);
                I_OnContecting?.OnContecting(this, other);
            }
        }
    }

    bool IsTargetOnlyCloseOne(Collider other)
    {
        var headDist = Vector3.Distance(rayStartPoint, other.transform.position);

        var dir = other.transform.position - transform.position;
        var hits = Physics.RaycastAll(rayStartPoint, dir, headDist);

        foreach (var hit in hits)
        {
            if (!hit.transform.Equals(other.transform))
            {
                if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Hand")
                    && hit.collider.gameObject.layer != LayerMask.NameToLayer("HandPlayer")
                    && hit.collider.transform != transform)
                {
                    var nowDist = Vector3.Distance(rayStartPoint, hit.point);
                    if (nowDist < headDist)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}
