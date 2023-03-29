using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjCollisionDetectorConnector_OnCollisionEnter
{
    void OnCollisionEnter(ObjCollisionDetector detector, Collision collision);
}
