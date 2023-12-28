using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class InteractionObjHolster : MonoBehaviour
{
    public enum State { Keeping, Holding, Non }
    public State holsterStateFor = State.Non;
    public bool IsUsing => interactionObjHolsterPositioners.Find(x => x.interactionObj != null);
    public List<InteractionObjHolsterPositioner> interactionObjHolsterPositioners = new List<InteractionObjHolsterPositioner>();
    public Dictionary<GameObject, InteractionObjHolsterPositioner> holserRemap { private set; get; } = new Dictionary<GameObject, InteractionObjHolsterPositioner>();
    public void SetPositioner()
    {
        interactionObjHolsterPositioners.Clear();
        holserRemap.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            var positioner = transform.GetChild(i).GetComponent<InteractionObjHolsterPositioner>();
            if (positioner != null)
            {
                interactionObjHolsterPositioners.Add(positioner);
                positioner.LoacInteractionItem();
                if (positioner.interactionObjPrefab != null)
                {
                    holserRemap.Add(positioner.interactionObjPrefab, positioner);
                }
            }
        }
    }

    public InteractionObjHolsterPositioner GetContainHolster(GameObject gameObject)
    {
        var targetObj = gameObject;
        if (holserRemap.ContainsKey(targetObj))
        {
            return holserRemap[targetObj];
        }

        return null;
    }

    public bool TryHold(InteractionObj obj)
    {
        if (IsUsing) return false;
        var positioner = GetContainHolster(obj.originalPrefab);
        if (positioner == null) return false;
        return positioner.TryHoldInteractionObj(obj);
    }

    public bool TryRemove(InteractionObj obj)
    {
        var positioner = GetContainHolster(obj.originalPrefab);
        if (positioner == null) return false;
        return positioner.TryRemoveInterationObj();
    }

    public List<InteractionObj> GetInteractionObj()
    {
        var list = new List<InteractionObj>();
        foreach (var positioner in interactionObjHolsterPositioners)
        {
            if (positioner.interactionObj != null)
            {
                list.Add(positioner.interactionObj);
            }
        }
        return list;
    }
}
