using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public static TargetManager instance { set; get; }
    public static bool IsExist { get { return instance != null; } }
    public PersonHandler personHandler;
    public int targetCount = 1;
    List<Person> TargetPerson = new List<Person>();
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

    IEnumerator Start()
    {
        yield return new WaitUntil(() => personHandler.isStartPass);

        TargetPerson = personHandler.SetEnemy(targetCount);
    }

    public int GetLeftTarget()
    {
        var left = 0;
        for (int i = 0; i < TargetPerson.Count; i++)
        {
            left += TargetPerson[i].NowAliveState == Person.AliveState.Alive ? 1 : 0;
        }
        return left;
    }
}
