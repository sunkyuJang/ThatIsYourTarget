using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonHandler : MonoBehaviour
{
    public List<Person> Persons { set; get; } = new List<Person>();
    public List<Material> belongKinds = new List<Material>();
    Material TargetColor { get { return belongKinds[0]; } }
    int NormalPersonStartFrom { get { return 1; } }
    public bool isStartPass = false;
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var person = transform.GetChild(i).GetComponent<Person>();
            person.SetBelongTo(belongKinds[Random.Range(NormalPersonStartFrom, belongKinds.Count)]);
            Persons.Add(person);
        }

        isStartPass = true;
    }

    public List<Person> SetEnemy(int enemyCount)
    {
        var nowPersonList = new List<Person>(0);
        for (int i = 0; i < enemyCount; i++)
        {
            var nowPerson = Persons[Random.Range(0, Persons.Count)];
            nowPerson.SetBelongTo(TargetColor);
            nowPersonList.Add(nowPerson);
        }

        return nowPersonList;
    }
}
