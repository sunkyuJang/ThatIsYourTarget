using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonAniStateModuleHandler : StateModuleHandler
{
    public PersonAniStateModuleHandler(Animator animator)
    {
        modules = PersonAniState.GetStatesList(animator);
    }
}
