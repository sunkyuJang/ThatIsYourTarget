using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjDetectorConnector_OnRemoved
{
    public void OnRemoved(ObjDetector detector, Collider collider);
}
