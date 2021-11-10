using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PersonModel : MonoBehaviour
{
    Person person;
    Animator animator;
    Rigidbody Rigidbody { set; get; } = null;
    RagDollHandler ragDollHandler;
    public NavMeshAgent NavMeshAgent { set; get; }
    public NavMeshObstacle navMeshObstacle { set; get; }
    public Renderer ModelRender { protected set; get; }
    public Transform threeDIconGroup;
    public enum ThreeD_IconList { Exclamation = 0, Question, SpeechBubble }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        ragDollHandler = GetComponent<RagDollHandler>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();

        navMeshObstacle.enabled = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            var nowObj = transform.GetChild(i).gameObject;
            if (nowObj.activeSelf)
            {
                ModelRender = nowObj.GetComponent<Renderer>();
                break;
            }
        }
    }

    public void SetAlertLevel(Person.AlertLevel level)
    {
        ResetAnimatorWeight();
        var index = -1;
        switch (level)
        {
            case Person.AlertLevel.Normal:
                {
                    index = 0;
                    break;
                }
            case Person.AlertLevel.Notice: index = 1; break;
            case Person.AlertLevel.Attack: index = 2; break;
            case Person.AlertLevel.Avoid: index = 3; break;
        }
        animator.SetLayerWeight(index, 1);
    }

    void ResetAnimatorWeight()
    {
        for (int i = 0; i < animator.layerCount; i++)
        {
            animator.SetLayerWeight(i, 0);
        }
    }

    public void SetNextPosition(Vector3 worldPosition)
    {
        NavMeshAgent.SetDestination(worldPosition);
    }

    public void SetPositionCorrectly(Vector3 worldPosition)
    {
        StartCoroutine(DoSetPositionCorrectly(worldPosition));
    }

    IEnumerator DoSetPositionCorrectly(Vector3 worldPosition)
    {
        var t = 0f;
        var maxT = 0.5f;
        var beforePosition = transform.position;
        while (t < maxT)
        {
            yield return new WaitForFixedUpdate();
            t += Time.fixedDeltaTime;
            var ratio = Mathf.InverseLerp(0, maxT, t);
            transform.position = Vector3.Lerp(beforePosition, worldPosition, ratio);
        }
    }

    private void FixedUpdate()
    {
        var degree = Mathf.Round(Vector3.Angle(transform.forward, NavMeshAgent.velocity.normalized));
        var cross = Vector3.Cross(transform.forward, NavMeshAgent.velocity.normalized);
        degree *= cross.y >= 0 ? 1 : -1;

        animator.SetFloat("WalkY", Mathf.Cos(degree * Mathf.Deg2Rad));
        animator.SetFloat("WalkX", Mathf.Sin(degree * Mathf.Deg2Rad));
    }
    public virtual void GetHit()
    {
        animator.enabled = false;
        ragDollHandler.TrunOnRigid(true);
        NavMeshAgent.enabled = false;
    }

    public void SetBelong(Material material)
    {
        ModelRender.material = material;
    }

    public void MakeLookAt(Vector3 dir)
    {
        if (NavMeshAgent.enabled)
            NavMeshAgent.isStopped = true;
        StartCoroutine(DoLookAtWithSpeed(dir));
    }
    IEnumerator DoLookAtWithSpeed(Vector3 dir)
    {
        //Roughly
        var startForward = transform.forward;
        var cross = Vector3.Cross(Vector3.up, startForward);
        var dot = Vector3.Dot(cross, dir);
        var isLeft = dot < 0;
        var rotateSpeed = 300f;
        var lastAngle = Vector3.Angle(transform.forward, dir);
        while (true)
        {
            transform.Rotate(isLeft ? Vector3.down : Vector3.up, rotateSpeed * Time.fixedDeltaTime);
            var nowAngle = Vector3.Angle(transform.forward, dir);
            if (nowAngle > lastAngle) break;
            else lastAngle = nowAngle;
            yield return new WaitForFixedUpdate();
        }

        //Correctly
        if (Vector3.Angle(transform.forward, dir) * Mathf.Rad2Deg > 3f)
        {
            var t = 0f;
            var maxT = 1f;
            startForward = transform.forward;
            while (t < maxT)
            {
                var ratio = Mathf.InverseLerp(0, maxT, t);
                transform.forward = Vector3.Lerp(startForward, dir, ratio);
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        yield return null;
    }

    public void SetSittingAnimation(int sittingLevel)
    {

        animator.SetInteger("SittingLevel", sittingLevel);
        animator.SetInteger("WalkAroundLevel", 0);
        MakeItFixed(true);
    }

    public void SetLookAroundAnimation()
    {
        animator.SetBool("LookAround", true);
        animator.SetInteger("WalkAroundLevel", 0);
    }
    public void SetIdleAnimation()
    {
        animator.SetBool("ShouldDoIdle", true);
        animator.SetInteger("WalkAroundLevel", 0);
    }

    public void SetWalkState(int walkLevel)
    {
        StartCoroutine(DoWalkAnimation(walkLevel));
    }

    IEnumerator DoWalkAnimation(int walkLevel)
    {
        var isStateWasIdle = animator.GetBool("ShouldDoIdle")
                                || animator.GetBool("LookAround");

        var isThisWasStop = NavMeshAgent.velocity == Vector3.zero;

        animator.SetInteger("WalkAroundLevel", walkLevel);

        animator.SetInteger("SittingLevel", 0);
        animator.SetBool("LookAround", false);
        animator.SetBool("ShouldTurn", false);
        animator.SetBool("ShouldDoIdle", false);

        if (!isStateWasIdle)
        {
            if (isThisWasStop)
            {
                yield return StartCoroutine(DoNavMeshAgentWork());
            }
        }
        MakeItFixed(false);
        NavMeshAgent.isStopped = false;
    }

    IEnumerator DoNavMeshAgentWork()
    {
        yield return new WaitForSeconds(1f);
    }

    public void HideAllThreeD_Icon()
    {
        for (int i = 0; i < threeDIconGroup.childCount; i++)
        {
            threeDIconGroup.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void ShowThreeD_Icon(ThreeD_IconList iconName)
    {
        HideAllThreeD_Icon();
        threeDIconGroup.Find(iconName.ToString()).gameObject.SetActive(true);
    }

    void MakeItFixed(bool IsNeedToFixed)
    {
        if (IsNeedToFixed)
        {
            NavMeshAgent.enabled = !IsNeedToFixed;
            navMeshObstacle.enabled = IsNeedToFixed;
        }
        else
        {
            navMeshObstacle.enabled = IsNeedToFixed;
            NavMeshAgent.enabled = !IsNeedToFixed;
        }
    }
}
