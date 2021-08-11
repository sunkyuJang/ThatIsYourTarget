using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjDetectorConnector_OnDetected
{
    public void OnDetected(ObjDetector detector, Collider collider);
}
