public class PersonStateMouleHandler : StateModuleHandler
{
    public PersonStateMouleHandler(Person person)
    {
        modules = PersonState.GetStatesList(person);
    }

    public PersonState GetModule(PersonState.StateKinds kinds) => GetModule(ConvertStateKindToInt(kinds)) as PersonState;
    public bool IsSameModule(PersonState.StateKinds kinds) => IsSameModule(ConvertStateKindToInt(kinds));
    private int ConvertStateKindToInt(PersonState.StateKinds kinds) => (int)kinds;
    public int GetPlayingModuleIndex() => playingModuleIndex;
}
