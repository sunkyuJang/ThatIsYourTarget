public class PersonStateModuleHandler : StateModuleHandler
{
    public PersonStateModuleHandler(Person person)
    {
        modules = PersonState.GetStatesList(person);
    }

    public PersonState GetModule(PersonState.StateKinds kinds) => GetModule(ConvertStateKindToInt(kinds)) as PersonState;
    public T GetModule<T>(PersonState.StateKinds kinds) where T : PersonState => GetModule(kinds) as T;
    public bool IsSameModule(PersonState.StateKinds kinds) => IsSameModule(ConvertStateKindToInt(kinds));
    private int ConvertStateKindToInt(PersonState.StateKinds kinds) => (int)kinds;
    public int GetPlayingModuleIndex() => playingModuleIndex;
    public PersonState GetPlayingModule() => GetModule((PersonState.StateKinds)GetPlayingModuleIndex());
}
