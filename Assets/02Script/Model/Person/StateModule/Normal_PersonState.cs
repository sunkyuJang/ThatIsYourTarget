public class Normal_PersonState : PersonState
{
    public Normal_PersonState(Person person) : base(person)
    {

    }
    protected override bool IsReadyForEnter()
    {
        return true;
    }
    public override void EnterToException() { }
    protected override void DoEnter()
    {
        person.SetOriginalAPH();
    }
    public override void Exit() { }
    public override void AfterAPHDone() { }
}
