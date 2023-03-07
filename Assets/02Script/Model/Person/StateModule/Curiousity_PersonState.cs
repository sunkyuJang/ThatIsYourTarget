using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curiousity_PersonState : PersonState
{
    Sensed_PersonState.PreparingData preparingData;
    int curiosityCnt = 0;
    const int MaxCuriosityCnt = 3;
    float warningTime = 0f;
    const float maxWarningTime = 3f;
    bool isAPHDone = false;
    Coroutine procCountingTime = null;
    public Curiousity_PersonState(Person person) : base(person) { }
    public void PrepareState(Sensed_PersonState.PreparingData param)
    {
        if (preparingData == null)
        {
            preparingData = param as Sensed_PersonState.PreparingData;
        }
        else if (preparingData.target.Equals(param.target))
        {
            curiosityCnt++;
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
        var fisrtAP = aph.GetNowActionPoint();

        // 하는 중. 
        //첫 애니메이션 작동시 model이 움직이면서 콜라이터의 connect 및 remove가 지속적으로 발생하는 것을 막기위해
        //척 애니메이션이 동작하는 동안 만큼은 connect 및 remove를 통해 입력된 값을 무시하도록 한다.
        fisrtAP.during;
        person.SetAPH(aph, AfterAPHDone);
        if (procCountingTime != null)
        {
            person.StopCoroutine(procCountingTime);
        }

        procCountingTime = person.StartCoroutine(CountingTime(preparingData.target));
    }
    IEnumerator IgnoreTime()
    {
        Application.
        PersonAniController.StateKind.Surprize
        yield return null;
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
        isAPHDone = false;
        warningTime = 0;
        procCountingTime = null;
        curiosityCnt = 0;
    }
    public override void AfterAPHDone()
    {
        isAPHDone = true;
        SetNormalState();
    }
}
