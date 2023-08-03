public class Sensed_PersonState : PersonState
{
    public Sensed_PersonState(Person person) : base(person) { }

    public override bool IsReady()
    {
        var playingModule = person.moduleHandler.GetPlayingModule();
        if (playingModule != null
            && !IsTargetModelSame(playingModule))
        {
            return true;
        }

        return false;
    }
    public override void EnterToException()
    {
        SetNormalState();
    }
    protected override void StartModule()
    {
        var dist = person.modelHandler.GetDistTo(prepareData.target.transform);
        var shouldAttack = dist < PrepareAttack_PersonState.attackDist;
        if (shouldAttack)
        {

        }
        else
        {
            var state = person.moduleHandler.GetModule(StateKinds.Curiousity);
            if (state != null)
            {
                (state as Curiousity_PersonState)?.prepareData = new PersonPrepareData(prepareData.target);
            }
        }
        SetState(dist < PrepareAttack_PersonState.attackDist ? StateKinds.PrepareAttack : StateKinds.Curiousity);
    }
}
