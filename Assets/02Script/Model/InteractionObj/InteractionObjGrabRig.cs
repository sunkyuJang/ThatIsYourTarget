using System.Collections;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public class InteractionObjGrabRig : MonoBehaviour
{
    public enum State { Holding, Using, Non }
    public State state = State.Non;
    public GameObject OriginalPrefab;
    public float weight = 0f;
    public float time = 0f;
    LimbIK[] Limb { set; get; }
    FingerRig fingerRig { set; get; }
    Coroutine coroutine { set; get; }
    public bool IsUsingThis { get { return weight > 0; } }
    private void Awake()
    {
        if (OriginalPrefab == null) Debug.Log("original Prefab is empty");

        Limb = GetComponents<LimbIK>();
        fingerRig = GetComponent<FingerRig>();
    }

    private void Start()
    {
        TurnOn_IK(false);
    }

    public bool IsSamePrefab(GameObject gameObject)
    {
        return OriginalPrefab == gameObject;
    }

    public void TurnOn_IK(bool turnOn)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(DoStartIK(turnOn));
    }

    IEnumerator DoStartIK(bool turnOn)
    {
        float startWeight = weight;
        float endWeight = turnOn ? 1f : 0f;
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            weight = Mathf.Lerp(startWeight, endWeight, elapsedTime / time);

            // Update IK weights here
            foreach (var limb in Limb)
            {
                limb.solver.IKPositionWeight = weight;
                limb.solver.IKRotationWeight = weight;
            }
            fingerRig.weight = weight;

            yield return null;
        }

        weight = endWeight; // Ensure final weight is set

        // Update final IK weights
        foreach (var limb in Limb)
        {
            limb.solver.IKPositionWeight = weight;
        }
        fingerRig.weight = weight;

        coroutine = null;
    }
}
