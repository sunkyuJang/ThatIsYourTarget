using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations;

public class ModelGrouper : MonoBehaviour
{
    [SerializeField] private GameObject originalPrefab;
    [SerializeField] private List<Model> models = new List<Model>();
    private Model representModel;
    private ObjPooler modelPooler;
    SkillManager skillManager;
    private void Awake()
    {
        representModel = originalPrefab.GetComponent<Model>();
        if (representModel == null || representModel.ActorTransform == null)
        {
            Debug.Log("The original prefab must be a registered prefab that follows the Model format.");
            representModel = null;
            return;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            var model = transform.GetChild(i).GetComponent<Model>();
            if (model != null)
                models.Add(model);
        }
    }

    private void Start()
    {
        if (representModel == null) return;

        modelPooler = ObjPoolerManager.Instance.GetPooler(originalPrefab);
        var attackingAnimationStateManager = AnimatorStateManager.Instance.GetAttackingStateManager(representModel.ActorTransform.GetComponent<Animator>().runtimeAnimatorController as AnimatorController);
        if (attackingAnimationStateManager != null)
        {
            skillManager = attackingAnimationStateManager.skillManager;
            RequestMakingSKillDetector();
        }
    }

    protected void RequestMakingSKillDetector()
    {
        if (skillManager != null)
        {
            skillManager.MakeSkillDetector(models.Count);
        }
    }
}
