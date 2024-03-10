using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public static TargetManager instance { set; get; }
    public static bool IsExist { get { return instance != null; } }
    public HumanHandler personHandler;
    public int targetCount = 1;
    List<Human> TargetPerson = new List<Human>();
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

    private void Start()
    {
        TargetPerson = personHandler.SetEnemy(targetCount);
    }

    public int GetLeftTarget()
    {
        var left = 0;
        for (int i = 0; i < TargetPerson.Count; i++)
        {
            left += (HumanState.StateKinds)TargetPerson[i].state != HumanState.StateKinds.Dead ? 1 : 0;
        }
        return left;
    }
}
