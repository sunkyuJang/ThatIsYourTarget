using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationEntry : MonoBehaviour
{
    public List<Person> targetPersonList = new List<Person>();
    Transform APGroup;
    public bool isStartingByForce;
    public bool isTalkingByVoice;
    public bool isRandomTarget;
    public int targetIncount = 0;
    private void Awake()
    {
        APGroup = transform.Find("APGroup");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PersonModel"))
        {
            print("1pass");
            for (int i = 0; i < targetPersonList.Count; i++)
            {
                var nowPersonName = targetPersonList[i].name;
                if (other.transform.parent.gameObject.name == nowPersonName)
                {
                    print("2pass");
                    targetIncount++;
                    SetConversationBuild(other.transform.parent.GetComponent<Person>());
                    break;
                }
            }
        }

        if (targetIncount == targetPersonList.Count)
        {
            StartConversation();
        }
    }

    void SetConversationBuild(Person target)
    {
        for (int i = 0; i < APGroup.childCount; i++)
        {
            var nowGroup = APGroup.GetChild(i).GetComponent<ActionPointHandler>();
            if (target.gameObject.name == nowGroup.gameObject.name)
            {
                nowGroup.GetActionPoint(0).during = -1;
                target.InterruptAPs(nowGroup);
            }
        }
    }

    void StartConversation()
    {
        for (int i = 0; i < APGroup.childCount; i++)
        {
            APGroup.GetChild(i).GetComponent<ActionPointHandler>().GetActionPoint(0).during = 0;
        }
    }
    // void SetConversationBuild()
    // {
    //     for (int i = 0; i < APGroup.childCount; i++)
    //     {
    //         var nowGroup = APGroup.GetChild(i).GetComponent<ActionPointHandler>();
    //         for (int targetIndex = 0; targetIndex < targetPersonList.Count; targetIndex++)
    //         {
    //             var nowTarget = targetPersonList[targetIndex];
    //             if (nowGroup.name.Equals(nowTarget.name))
    //             {
    //                 print(true);
    //                 nowTarget.InterruptAPs(nowGroup);
    //                 break;
    //             }
    //         }
    //     }
    // }
}
