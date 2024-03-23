using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityHandlerTester : MonoBehaviour
{
    AbilityHandler abilityHandler;
    public EffectingToRemainingAbility effectingToRemainingAbility;
    private void Awake()
    {
        abilityHandler = GetComponent<AbilityHandler>();
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        abilityHandler.SetRemainingAbility(effectingToRemainingAbility);
    }
}
