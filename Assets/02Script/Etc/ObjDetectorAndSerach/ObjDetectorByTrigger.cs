using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjDetectorByRay : ObjDetector
{
    public float Range { set; get; }
    private void Start()
    {
        SetInterface();
    }
}
