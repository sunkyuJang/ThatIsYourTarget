using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonStateMouleHandler : StateModuleHandler
{
    public PersonStateMouleHandler(Person person)
    {
        modules = PersonState.GetStatesList(person);
    }
}
