using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjMover : MonoBehaviour, IObjDetectorConnector_OnDetected
{
    public GameObject MovePosition;
    public void OnDetected(ObjDetector detector, Collider collider)
    {
        transform.position = MovePosition.transform.position;
    }
}
