using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FovSensor : ObjDetector
{
    public Vector3 rayStatPoint { get { return transform.position + transform.forward * 0.5f; } }
    protected override void OnTriggerStay(Collider other)
    {
        if (IsFind(other))
        {
            var headDist = Vector3.Distance(rayStatPoint, other.transform.position);

            var hits = Physics.RaycastAll(rayStatPoint, other.transform.position);
            print("isIN");
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Hand")
                    && hit.collider.gameObject.layer != LayerMask.NameToLayer("HandPlayer"))
                {
                    var nowDist = Vector3.Distance(rayStatPoint, hit.transform.position);
                    if (nowDist < headDist)
                    {
                        print(hit.collider.name);
                        return;
                    }
                }
            }

            print(true);
            Debug.DrawLine(rayStatPoint, other.transform.position, Color.red, 2f);
        }
    }
}
