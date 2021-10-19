using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationEntry : MonoBehaviour
{
    public List<Person> targetPersonList = new List<Person>();
    List<ActionPointHandler> APHList = new List<ActionPointHandler>();
    public bool isStartingByForce;
    public bool isTalkingByVoice;
    public bool isRandomTarget;
    public int targetIncount = 0;
    public bool isStartConversation = false;
    private void Awake()
    {
        var APGroup = transform.Find("APGroup");
        for (int i = 0; i < APGroup.childCount; i++)
        {
            APHList.Add(APGroup.GetChild(i).GetComponent<ActionPointHandler>());
        }
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

        if (!isStartConversation
            && targetIncount == targetPersonList.Count)
        {
            isStartConversation = true;
            StartCoroutine(WaitForStandingOnStartPosition());
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
        for (int i = 0; i < APHList.Count; i++)
        {
            var nowGroup = APHList[i];
            if (target.gameObject.name == nowGroup.gameObject.name)
            {
                target.ChangeAPHandler(nowGroup);
            }
        }
    }

    IEnumerator WaitForStandingOnStartPosition()
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

        StartTalk();
        yield return null;
    }
    void StartTalk()
    {
        for (int i = 0; i < APHList.Count; i++)
        {
            APHList[i].WaitForStartToNext(false);
        }
        StartCoroutine(DoStartTalk());
    }

    IEnumerator DoStartTalk()
    {
        do
        {
            var nowPerson = targetPersonList[0];
            targetPersonList.Remove(nowPerson);

            nowPerson.model.ShowThreeD_Icon(PersonModel.ThreeD_IconList.SpeechBubble);
            var timeData = TimeCounter.Instance.SetTimeCounting(Random.Range(1, 5), 1f, nowPerson.model.gameObject);
            yield return new WaitUntil(() => !timeData.IsCounting);
            TimeCounter.Instance.RemoveProcessCounting(timeData);
            if (targetPersonList.Count > 1)
                targetPersonList.Insert(Random.Range(1, targetPersonList.Count), nowPerson);
            else
                targetPersonList.Add(nowPerson);

            nowPerson.model.HideAllThreeD_Icon();
        } while (APHList.Find(x => !x.IsReachedToEnd));

        MakeReset();
        yield return null;
    }

    void MakeReset()
    {
        for (int i = 0; i < targetPersonList.Count; i++)
        {
            var nowTarget = targetPersonList[i];
            nowTarget.ChangeAPHandler(null);
        }

        for (int i = 0; i < APHList.Count; i++)
        {
            APHList[i].IsReachedToEnd = false;
            APHList[i].WaitForStartToNext(true);
            APHList[i].ResetIndex();
        }

        isStartConversation = false;
        targetIncount = 0;
    }
}
