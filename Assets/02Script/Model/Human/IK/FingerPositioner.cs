using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InteractionFingerPositioner : MonoBehaviour
{
    public InteractionFingerPositionHandler.HandPosition handPosition = InteractionFingerPositionHandler.HandPosition.LeftHand;
    [SerializeField] private Transform thumb = null;
    [SerializeField] private Transform index = null;
    [SerializeField] private Transform finger = null;

    public Transform GetThumb { get { return thumb; } }
    public Transform GetIndex { get { return index; } }
    public Transform GetFinger { get { return finger; } }
}
