using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curiousity_PersonState : PersonState
{
    Sensed_PersonState.PreparingData preparingData;
    int curiosityCnt = 0;
    const int MaxCuriosityCnt = 0;
    float warningTime = 0f;
    const float maxWarningTime = 3f;
    bool isAPHDone = false;
    Coroutine procCountingTime = null;
    public void PrepareState(Sensed_PersonState.PreparingData param)
    {
        if (preparingData == null)
        {
            preparingData = param as Sensed_PersonState.PreparingData;
        }
        else
        {

        }

        SetState(StateKinds.Curiousity);
    }

    public override bool IsReadyForEnter()
    {
        return preparingData != null &&
                preparingData.target != null &&
                curiosityCnt < MaxCuriosityCnt;
    }
    public override void EnterToException()
    {
        if (curiosityCnt < MaxCuriosityCnt)
        {
            SetNormalState();
        }
        else
        {
            SetState(StateKinds.Attack);
        }
    }
    public override void Enter()
    {
        var aph = GetCuriousityAPH(preparingData.target);
        person.SetAPH(aph, AfterAPHDone);
        if (procCountingTime != null)
        {
            person.StopCoroutine(procCountingTime);
        }

        procCountingTime = person.StartCoroutine(CountingTime(preparingData.target));
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
                    var dist = person.GetDistTo(hit.transform);
                    if (dist < Attack_PersonState.attackDist)
                    {
                        warningTime += maxWarningTime;
                    }

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

        procCountingTime = null;
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
        procCountingTime = null;
    }
    public override void AfterAPHDone()
    {
        isAPHDone = true;
        SetNormalState();
    }
}
