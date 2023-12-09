using UnityEditor;
using UnityEngine;

public class InteractionObjHolsterPositioner : MonoBehaviour
{
    public GameObject interactionObjPrefab;
    public InteractionObj interactionObj;
    public bool CanHold(GameObject target)
        => interactionObjPrefab == PrefabUtility.GetOriginalSourceRootWhereGameObjectIsAdded(target);
    public InteractionObj GetInteractionObj() { return interactionObj; }
    public void LoacInteractionItem()
    {
        interactionObj = transform.GetComponentInChildren<InteractionObj>();
    }
    public bool TryHoldInteractionObj(InteractionObj targetInteractionObj)
    {
        if (!CanHold(targetInteractionObj.gameObject)) return false;
        var targetTransform = targetInteractionObj.transform;
        targetTransform.SetParent(transform);
        targetTransform.localPosition = Vector3.one;
        transform.localRotation = Quaternion.identity;
        interactionObj = targetInteractionObj;
        return true;
    }

    public bool TryRemoveInterationObj()
    {
        if (GetInteractionObj() == null) return false;
        interactionObj = null;

        return true;
    }
}
