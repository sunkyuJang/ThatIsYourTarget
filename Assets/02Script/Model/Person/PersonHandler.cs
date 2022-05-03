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
            //person.SetBelongTo(belongKinds[Random.Range(NormalPersonStartFrom, belongKinds.Count)]);
            Persons.Add(person);
        }

        isStartPass = true;
    }

    public List<Person> SetEnemy(int enemyCount)
    {
        var nowPersonList = new List<Person>();
        for (int i = 0; i < enemyCount; i++)
        {
            var index = Random.Range(0, Persons.Count);
            var nowPerson = Persons[index];
            Persons.RemoveAt(index);
            nowPerson.SetBelongTo(TargetColor);
            nowPersonList.Add(nowPerson);
        }
        Persons.AddRange(nowPersonList);

        return nowPersonList;
    }
}
