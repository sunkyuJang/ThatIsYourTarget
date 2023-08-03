using System.Collections;
using UnityEngine;

public class Curiousity_PersonState : PersonState
{
    int curiosityCnt = 0;
    const int MaxCuriosityCnt = 3;
    float warningTime = 0f;
    const float maxWarningTime = 3f;
    bool isAPHDone = false;
    Coroutine procCountingTime = null;
    Coroutine procCountingIgnoreTime = null;
    public Curiousity_PersonState(Person person) : base(person) { }
    public override bool IsReady()
    {
        return prepareData != null &&
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
            SetState(StateKinds.PrepareAttack, new PersonPrepareData(prepareData.target));
        }
    }
    protected override void StartModule()
    {
        var aph = GetCuriousityAPH(prepareData.target.transform);
        // 하는 중. 
        //첫 애니메이션 작동시 model이 움직이면서 콜라이터의 connect 및 remove가 지속적으로 발생하는 것을 막기위해
        //척 애니메이션이 동작하는 동안 만큼은 connect 및 remove를 통해 입력된 값을 무시하도록 한다.
        if (procCountingIgnoreTime != null) return;

        procCountingIgnoreTime = person.StartCoroutine(IgnoreTime(aph));
        person.SetAPH(aph, AfterAPHDone);
        if (procCountingTime != null)
        {
            person.StopCoroutine(procCountingTime);
        }

        procCountingTime = person.StartCoroutine(CountingTime(prepareData.target.transform));
    }
    IEnumerator IgnoreTime(ActionPointHandler aph)
    {
        yield return new WaitUntil(() => aph.index > 0);
        procCountingIgnoreTime = null;
    }

    IEnumerator CountingTime(Transform target)
    {
        while (!isAPHDone)
        {
            var isHit = person.modelHandler.IsInSight(target);
            if (isHit)
            {
                if (target.CompareTag(Player.playerTag))
                {
                    var dist = person.modelHandler.GetDistTo(target);
                    if (dist < PrepareAttack_PersonState.attackDist)
                    {
                        warningTime += maxWarningTime;
                    }

                    warningTime += Time.fixedDeltaTime;
                }
            }


            if (warningTime > maxWarningTime)
            {
                SetState(StateKinds.PrepareAttack, new PersonPrepareData(prepareData.target));
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
        person.SetAPs(aph.actionPoints[0], target, PersonAniState.StateKind.Surprize, true, 0, false, true);
        person.SetAPs(aph.actionPoints[1], target, PersonAniState.StateKind.LookAround, true, 0, true, false);

        return aph;
    }

    public override void Exit()
    {
        isAPHDone = false;
        warningTime = 0;
        procCountingTime = null;
        curiosityCnt = 0;
    }
    protected override StateKinds DoAfterDone(out PersonPrepareData prepareData)
    {
        prepareData = null;
        isAPHDone = true;
        return StateKinds.Normal;
    }
}
