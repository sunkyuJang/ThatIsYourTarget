using UnityEngine;

public class Sensed_PersonState : PersonState
{
    private PreparingData preparingData;
    public Sensed_PersonState(Person person) : base(person) { }
    public void PrepareState(PreparingData param)
    {
        preparingData = param as PreparingData;
    }

    public override bool IsReadyForEnter()
    {
        return preparingData != null && preparingData.target != null;
    }
    public override void EnterToException()
    {
        SetNormalState();
    }
    protected override void DoEnter()
    {
        var dist = person.modelHandler.GetDistTo(preparingData.target);
        var shouldAttack = dist < Attack_PersonState.attackDist;
        if (shouldAttack)
        {

        }
        else
        {
            var state = GetState(StateKinds.Curiousity);
            if (state != null)
            {
                (state as Curiousity_PersonState)?.PrepareState(preparingData);
            }
        }
        SetState(dist < Attack_PersonState.attackDist ? StateKinds.Attack : StateKinds.Curiousity);
    }
    public override void Exit() { preparingData = null; }

    public override void AfterAPHDone() { }

    public class PreparingData
    {
        public Transform target { private set; get; }
        public bool isInSight { private set; get; }

        public PreparingData(Transform target, bool isInSight)
        {
            this.target = target;
            this.isInSight = isInSight;
        }
    }
}
