using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curiousity_PersonState : PersonState
{
    public Transform target;
    float warningTime = 0f;
    float maxWarningTime = 3f;
    bool isAPHDone = false;
    Coroutine procCountingTime = null;
    public override void Enter()
    {
        if (target == null)
            SetNormalState();

        var aph = GetCuriousityAPH(target);
        person.SetAPH(aph, AfterAPHDone);
        if (procCountingTime != null)
            person.StopCoroutine(procCountingTime);

        procCountingTime = person.StartCoroutine(CountingTime(target));
    }

    IEnumerator CountingTime(Transform target)
    {
        while (!isAPHDone)
        {
            var hits = person.GetAllRayHIts(target);
            if (hits.Length == 1)
            {
                var hit = hits[0];
                if (hit.transform.CompareTag(Player.playerTag))
                {
                    warningTime += Time.fixedDeltaTime;
                }
            }

            if (warningTime > maxWarningTime)
            {
                SetState(StateKinds.Attack);
                break;
            }

            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    private ActionPointHandler GetCuriousityAPH(Transform target)
    {
        var aph = person.GetNewAPH(2);
        person.SetAPs(aph.actionPoints[0], target, PersonAniController.StateKind.Surprize, true);
        person.SetAPs(aph.actionPoints[1], target, PersonAniController.StateKind.LookAround, true, 0, false, false);

        return aph;
    }

    public override void Exit()
    {
        target = null;
        isAPHDone = false;
        warningTime = 0;
    }
    protected override void Update() { }
    public void SetChangeForThisState(Transform transform)
    {
        this.target = transform;
        SetState(StateKinds.Curiousity);
    }

    public override void AfterAPHDone()
    {
        isAPHDone = true;
        SetNormalState();
    }
}
