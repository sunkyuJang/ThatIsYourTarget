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
        if (isRandomTarget)
        {

        }
        else
        {
            CheckFixedTarget(other);
        }

        if (targetIncount == targetPersonList.Count)
        {
            StartCoroutine(DoConversation());
        }
    }

    void CheckFixedTarget(Collider other)
    {
        if (other.CompareTag("PersonModel"))
        {
            for (int i = 0; i < targetPersonList.Count; i++)
            {
                var nowPersonName = targetPersonList[i].name;
                if (other.transform.parent.gameObject.name == nowPersonName)
                {
                    targetIncount++;
                    SetConversationBuild(other.transform.parent.GetComponent<Person>());
                    break;
                }
            }
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
                target.InterruptAPHandler(nowGroup);
            }
        }
    }

    IEnumerator DoConversation()
    {
        var canPass = false;
        while (!canPass)
        {
            canPass = true;
            for (int i = 0; i < targetPersonList.Count; i++)
            {
                if (!targetPersonList[i].IsStandingOnPosition)
                {
                    canPass = false;
                    break;
                }
            }
            yield return new WaitForFixedUpdate();
        }

        StartConversation();
        yield return null;
    }
    void StartConversation()
    {
        for (int i = 0; i < APGroup.childCount; i++)
        {
            APGroup.GetChild(i).GetComponent<ActionPointHandler>().GetActionPoint(0).during = 0;
        }
    }
}
