using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Threading;
using System;

public class InteractionObjHolsterHandler : MonoBehaviour
{
    // keeping != On Hands
    // Holding == On Hands with stay idle 
    // Using ==  OnHands with other stat
    // After Remapping, this list will not use.
    [SerializeField][ReadOnly] private List<InteractionObjHolster> Keepingholsters = new List<InteractionObjHolster>();
    [SerializeField][ReadOnly] private List<InteractionObjHolster> Holdingholsters = new List<InteractionObjHolster>();
    [SerializeField][ReadOnly] private InteractionObjGrabRigHandler grabRigHandler;
    [SerializeField] private bool shouldMatch = true;

    // Remap
    private Dictionary<GameObject, HolsterHandlerPair> HolsterPairs { set; get; } = new Dictionary<GameObject, HolsterHandlerPair>();
    public HolsterHandlerPair GetHolsterHandlerPair(GameObject gameObject)
    {
        var originalPrefab = gameObject;
        return HolsterPairs.ContainsKey(originalPrefab) ? HolsterPairs[originalPrefab] : null;
    }
    [SerializeField]
    private List<InteractionObj> interactionObjs = new List<InteractionObj>();

    private void Awake()
    {
        if (TrySetAllHolsterList())
        {
            RemappingAllHandler();
        }
        else
        {
            Debug.Log("somthing wrong when remapping");
        }
    }
    public bool TrySetAllHolsterList()
    {
        Keepingholsters.Clear();
        Holdingholsters.Clear();
        HolsterPairs.Clear();

        var isSucced = true;
        var holsterList = transform.GetComponentsInChildren<InteractionObjHolster>().ToList();
        holsterList.ForEach(x =>
        {
            if (x.holsterStateFor == InteractionObjHolster.State.Non)
            {
                Debug.LogError("Holster State Shouldnt Non");
                isSucced = false;
            }
            else
            {
                var target = x.holsterStateFor == InteractionObjHolster.State.Keeping ? Keepingholsters : Holdingholsters;
                x.SetPositioner();
                target.Add(x);
            }
        });

        grabRigHandler = transform.GetComponentInChildren<InteractionObjGrabRigHandler>();
        if (grabRigHandler == null)
        {
            isSucced = false;
            Debug.LogError("cantFind GrabRigHandler");
        }
        else
        {
            grabRigHandler.SetGrabRigs();
        }

        return isSucced;
    }

    public void RemappingAllHandler()
    {
        // remapping 
        Dictionary<GameObject, KeyValuePair<InteractionObjHolsterPositioner, InteractionObjHolster>> keepingHolsters = new Dictionary<GameObject, KeyValuePair<InteractionObjHolsterPositioner, InteractionObjHolster>>();
        Dictionary<GameObject, KeyValuePair<InteractionObjHolsterPositioner, InteractionObjHolster>> holdingHolsters = new Dictionary<GameObject, KeyValuePair<InteractionObjHolsterPositioner, InteractionObjHolster>>();
        var objGrabRigs = grabRigHandler.grabRigs.ToList();

        for (InteractionObjHolster.State state = InteractionObjHolster.State.Keeping; state <= InteractionObjHolster.State.Non; state++)
        {
            var originalHolsters = state == InteractionObjHolster.State.Keeping ? Keepingholsters : Holdingholsters;
            var remappingHolsters = state == InteractionObjHolster.State.Keeping ? keepingHolsters : holdingHolsters;
            if (originalHolsters != null)
            {
                foreach (var holster in originalHolsters)
                {
                    foreach (var obj in holster.holserRemap.Keys)
                    {
                        remappingHolsters[obj] = new KeyValuePair<InteractionObjHolsterPositioner, InteractionObjHolster>(holster.holserRemap[obj], holster);
                    }
                }
            }
        }

        // matching
        foreach (var key in keepingHolsters.Keys)
        {
            if (holdingHolsters.ContainsKey(key))
            {
                var grabRig = objGrabRigs.FindAll(x => x.IsSamePrefab(key));

                HolsterPairs[key] = new HolsterHandlerPair(keepingHolsters[key].Value, holdingHolsters[key].Value, grabRig);

                if (grabRig != null)
                {
                    grabRig.ForEach(x => objGrabRigs.Remove(x));
                }
            }
        }

        if (shouldMatch && (keepingHolsters.Count != holdingHolsters.Count))
        {
            Debug.LogError("holster isn't match");
        }

        LoadAllItemFromHandler();
    }

    public void LoadAllItemFromHandler()
    {
        interactionObjs.Clear();
        Keepingholsters.ForEach(x => { var objList = x.GetInteractionObj(); if (objList != null) objList.ForEach(j => interactionObjs.Add(j)); });
        Holdingholsters.ForEach(x => { var objList = x.GetInteractionObj(); if (objList != null) objList.ForEach(j => interactionObjs.Add(j)); });
    }

    public List<T> GetInteractionObj<T>() where T : InteractionObj
    {
        var list = new List<T>();
        interactionObjs.ForEach(x =>
        {
            if (x is T)
            {
                list.Add(x as T);
            }
        });
        return list;
    }

    public bool SetInteractionObj(InteractionObj interactionObj, InteractionObjHolster.State state)
    {
        if (interactionObjs.Contains(interactionObj)) return false;
        var pair = GetHolsterHandlerPair(interactionObj.originalPrefab);
        if (pair == null) return false;
        if (pair.IsUsing) return false;

        var isSucced = pair.GetHandlerByState(state).TryHold(interactionObj);
        return isSucced;
    }

    public bool SetKeep(InteractionObj interactionObj)
    {
        return TrySwitchToTargetHandler(interactionObj, InteractionObjHolster.State.Keeping, InteractionObjGrabRig.State.Non, out HolsterHandlerPair pair);
    }

    public bool SetHold(InteractionObj interactionObj, InteractionObjGrabRig.State grabbingState)
    {
        var isSucced = TrySwitchToTargetHandler(interactionObj, InteractionObjHolster.State.Holding, grabbingState, out HolsterHandlerPair pair);
        if (!isSucced) return false;

        pair.TurnOnRig(grabbingState);

        return true;
    }

    public InteractionObjGrabRig.State GetHoldingState(InteractionObj interactionObj)
    {
        var pair = GetHolsterHandlerPair(interactionObj.originalPrefab);
        return pair.GetTurnOnRigState;
    }

    bool TrySwitchToTargetHandler(InteractionObj interactionObj, InteractionObjHolster.State targetState, InteractionObjGrabRig.State targetGrabbingState, out HolsterHandlerPair pair)
    {
        var nowState = targetState == InteractionObjHolster.State.Keeping ? InteractionObjHolster.State.Holding : InteractionObjHolster.State.Keeping;
        pair = GetHolsterHandlerPair(interactionObj.originalPrefab);
        if (pair == null) return false;
        var nowHandler = pair.GetHandlerByState(nowState);
        var targetHandler = pair.GetHandlerByState(targetState);
        if (targetHandler.IsUsing)
        {
            if (targetState == InteractionObjHolster.State.Holding && targetGrabbingState != InteractionObjGrabRig.State.Non)
            {
                if (pair.GetTurnOnRigState != InteractionObjGrabRig.State.Non &&
                    pair.GetTurnOnRigState != targetGrabbingState)
                {
                    return true;
                }
            }
            Debug.Log("target holster is already using.");
            return false;
        }
        if (!nowHandler.IsUsing)
        {
            Debug.Log("now handler is not using.");
            return false;
        }

        var isSucced = nowHandler.TryRemove(interactionObj);
        if (!isSucced) return false;

        return targetHandler.TryHold(interactionObj);
    }

    [Serializable]
    public class HolsterHandlerPair
    {
        public GameObject originalPrefab;
        public InteractionObjHolster KeepingHolsterHandler { private set; get; }
        public InteractionObjHolster HoldingHolsterHandler { private set; get; }
        public List<InteractionObjGrabRig> GrabRig { set; get; }
        public InteractionObjHolster GetHandlerByState(InteractionObjHolster.State state) { return state == InteractionObjHolster.State.Keeping ? KeepingHolsterHandler : HoldingHolsterHandler; }
        public bool IsUsing { get => KeepingHolsterHandler.IsUsing || HoldingHolsterHandler.IsUsing; }
        public InteractionObjGrabRig.State GetTurnOnRigState
        {
            get
            {
                var state = GrabRig.Find(x => x.IsUsingThis);
                if (state == null) return InteractionObjGrabRig.State.Non;
                else
                {
                    return state.state;
                }
            }
        }
        public void TurnOnRig(InteractionObjGrabRig.State grabbingState)
        {
            GrabRig.ForEach(x =>
            {
                x.TurnOn_IK(x.state == grabbingState);
            });
        }
        public HolsterHandlerPair(
            InteractionObjHolster keepingholsterHandler,
            InteractionObjHolster holdingHolsterHandler,
            List<InteractionObjGrabRig> grabRig)
        {
            KeepingHolsterHandler = keepingholsterHandler;
            HoldingHolsterHandler = holdingHolsterHandler;
            GrabRig = grabRig;
        }
    }
}

[CustomEditor(typeof(InteractionObjHolsterHandler))]
public class InteractionObjTotalManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draw the default inspector

        InteractionObjHolsterHandler script = (InteractionObjHolsterHandler)target;

        if (GUILayout.Button("Set Components"))
        {
            if (script.TrySetAllHolsterList())
            {
                script.RemappingAllHandler();
            }
        }
    }
}