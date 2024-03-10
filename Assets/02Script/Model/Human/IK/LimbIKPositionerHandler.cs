using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LimbIKPositionerHandler : MonoBehaviour
{
    public SerializedDictionary<GameObject, InteractionObjLimbIKHandPositioner> eachLimbIKPositioner = new SerializedDictionary<GameObject, InteractionObjLimbIKHandPositioner>();
    public void SetLimbIK()
    {
        eachLimbIKPositioner.Clear();
        var lists = GetComponentsInChildren<InteractionObjLimbIKHandPositioner>();
        foreach (var ik in lists)
        {
            eachLimbIKPositioner.Add(ik.OriginalPrefab, ik);
        }
    }
}
