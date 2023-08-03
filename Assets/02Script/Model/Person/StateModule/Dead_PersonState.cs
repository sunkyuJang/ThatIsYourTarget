public class Dead_PersonState : PersonState
{
    public Dead_PersonState(Person person) : base(person)
    {

    }
    public override bool IsReady()
    {
        return true;
    }
    public override void EnterToException() { }
    protected override void StartModule()
    {
        person.modelHandler.SetDead();
    }
    public override void Exit() { }
}
