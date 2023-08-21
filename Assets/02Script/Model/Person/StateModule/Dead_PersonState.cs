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
        person.modelPhysicsHandler.SetDead();
    }
    public override void Exit() { }
}
