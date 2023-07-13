using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonStateMouleHandler : StateModuleHandler
{
    public PersonStateMouleHandler(Person person)
    {
        modules = PersonState.GetStatesList(person);
    }

    public PersonState GetModule(PersonState.StateKinds kinds) => GetModule((int)kinds) as PersonState;
}
