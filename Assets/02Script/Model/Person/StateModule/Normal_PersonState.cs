public class Normal_PersonState : PersonState
{
    public Normal_PersonState(Person person) : base(person)
    {

    }
    public override bool IsReady()
    {
        return true;
    }
    public override void EnterToException() { }
    protected override void StartModule()
    {
        SetAPH();
    }
    public override void Exit() { }
}
