public class Hit_PersonState : PersonState
{
    public Hit_PersonState(Person person) : base(person) { }
    public override bool IsReady()
    {
        return prepareData != null;
    }
    public override void EnterToException() { }
    protected override void StartModule()
    {
        person.SetOriginalAPH();
    }
    public override void Exit() { }
}
