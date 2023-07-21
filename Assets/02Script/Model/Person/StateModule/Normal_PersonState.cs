public class Normal_PersonState : PersonState
{
    public Normal_PersonState(Person person) : base(person)
    {

    }
    public override bool IsReadyForEnter()
    {
        return true;
    }
    public override void EnterToException() { }
    protected override void DoEnter()
    {
        person.SetOriginalAPH();
    }
    public override void Exit() { }
}
