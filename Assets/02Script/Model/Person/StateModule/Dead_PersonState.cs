public class Dead_PersonState : PersonState
{
    public Dead_PersonState(Person person) : base(person)
    {

    }
    protected override bool IsReadyForEnter()
    {
        return true;
    }
    public override void EnterToException() { }
    protected override void DoEnter()
    {
        person.modelHandler.SetDead();
    }
    public override void Exit() { }
    public override void AfterAPHDone() { }
}
