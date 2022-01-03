using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroadCasterForPerson : MonoBehaviour
{
    [HideInInspector]
    public BroadCasterForPerson instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void BroadCastWithList(List<Person> persons, Person.AlertLevel alertLevel)
    {

    }

    class BroadCasterEntity
    {
        public List<Person> persons = new List<Person>();
    }
}
