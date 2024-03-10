using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JExtentioner;

public class InteractionFingerPositionHandler : MonoBehaviour
{
    public enum HandPosition { LeftHand, RightHand }
    private List<InteractionFingerPositioner> interactionFingerPositioners = new List<InteractionFingerPositioner>(2);
    public InteractionFingerPositioner GetInteractionFingerPositioner(HandPosition handPosition) => interactionFingerPositioners[(int)handPosition];
    private void Awake()
    {
        var positioners = GetComponentsInChildren<InteractionFingerPositioner>();
        for (int i = 0; i < EnumExtentioner.GetEnumSize<HandPosition>(); i++)
        {
            var positioner = positioners[i];
            var handPosition = positioner.handPosition == HandPosition.LeftHand ? (int)HandPosition.LeftHand : (int)HandPosition.RightHand;
            interactionFingerPositioners[handPosition] = positioner;
        }
    }
}
