using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class AniController : MonoBehaviour
{
    protected RagDollHandler ragDollHandler { set; get; }
    protected ModelPhysicsController modelPhysicsController;
    protected Animator animator;
    public ChildAnimatorState GetState(string stateName)
    {
        var aniState = (animator.runtimeAnimatorController as AnimatorController).layers;
        foreach (AnimatorControllerLayer layer in aniState)
        {
            foreach (ChildAnimatorState state in layer.stateMachine.states)
            {
                if (state.state.name == stateName)
                    return state;
            }
        }

        return new ChildAnimatorState();
    }
    public float GetLength(string stateName)
    {
        var state = GetState(stateName);
        if (state.state != null)
        {
            var speed = state.state.speed;
            var motionName = state.state.motion.name;
            var clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == motionName)
                {
                    return clip.length / speed;
                }
            }
        }

        return 0;
    }
    protected void Awake()
    {
        ragDollHandler = GetComponent<RagDollHandler>();
        modelPhysicsController = GetComponent<ModelPhysicsController>();
        animator = GetComponent<Animator>();
    }

    protected void Start()
    {
        StartCoroutine(DoWalking());
    }

    IEnumerator DoWalking()
    {
        while (true)
        {
            if (modelPhysicsController.naviController.navMeshAgent != null)
            {
                if (modelPhysicsController.naviController.navMeshAgent.isOnNavMesh
                    && !modelPhysicsController.naviController.navMeshAgent.isStopped)
                {
                    var NavMeshAgent = modelPhysicsController.naviController.navMeshAgent;

                    var degree = Mathf.Round(Vector3.Angle(transform.forward, NavMeshAgent.velocity.normalized));
                    var cross = Vector3.Cross(transform.forward, NavMeshAgent.velocity.normalized);
                    degree *= cross.y >= 0 ? 1 : -1;

                    animator.SetFloat("WalkY", Mathf.Cos(degree * Mathf.Deg2Rad));
                    animator.SetFloat("WalkX", Mathf.Sin(degree * Mathf.Deg2Rad));
                }
                else
                {
                    animator.SetFloat("WalkY", 0f);
                    animator.SetFloat("WalkX", 0f);
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
    public virtual void StartAni(ActionPoint actionPoint, bool shouldReturnAP = false) { }
}
