using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityListener : MonoBehaviour
{
    public AbilityHandler abilityHandler { set; get; }
    private void Start()
    {
        if (abilityHandler == null)
        {
            Debug.Log("Damage controller shouldnt null");
        }
    }
}
