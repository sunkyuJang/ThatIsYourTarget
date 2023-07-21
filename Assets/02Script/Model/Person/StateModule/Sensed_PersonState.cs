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
        //var canEnteredStateList = new List<PersonState.StateKinds>() { StateKinds.Normal, };
        var isAllowedState = person.moduleHandler.IsSameModule(StateKinds.Normal);
        return isAllowedState && preparingData != null && preparingData.target != null;
    }
    public override void EnterToException()
    {
        SetNormalState();
    }
    protected override void DoEnter()
    {
        var dist = person.modelHandler.GetDistTo(preparingData.target);
        var shouldAttack = dist < PrepareAttack_PersonState.attackDist;
        if (shouldAttack)
        {

        }
        else
        {
            var state = person.moduleHandler.GetModule(StateKinds.Curiousity);
            if (state != null)
            {
                var curiousity_PrepareData = new Curiousity_PersonState.PreparingData(preparingData.target, preparingData.isInSight);
                (state as Curiousity_PersonState)?.PrepareState(curiousity_PrepareData);
            }
        }
        SetState(dist < PrepareAttack_PersonState.attackDist ? StateKinds.PrepareAttack : StateKinds.Curiousity);
    }
    public override void Exit() { preparingData = null; }

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
