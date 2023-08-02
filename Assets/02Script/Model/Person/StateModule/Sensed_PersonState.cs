public class Sensed_PersonState : PersonState
{
    public Sensed_PersonState(Person person) : base(person) { }

    public override bool IsReadyForEnter()
    {
        var isAllowedState = person.moduleHandler.IsSameModule(StateKinds.Normal);
        return isAllowedState && targetModel != null;
    }
    public override void EnterToException()
    {
        SetNormalState();
    }
    protected override void DoEnter()
    {
        var dist = person.modelHandler.GetDistTo(targetModel.transform);
        var shouldAttack = dist < PrepareAttack_PersonState.attackDist;
        if (shouldAttack)
        {

        }
        else
        {
            var state = person.moduleHandler.GetModule(StateKinds.Curiousity);
            if (state != null)
            {
                (state as Curiousity_PersonState)?.SetPrepareData(new PrepareData(targetModel));
            }
        }
        SetState(dist < PrepareAttack_PersonState.attackDist ? StateKinds.PrepareAttack : StateKinds.Curiousity);
    }
    public override void Exit() { targetModel = null; }
}
