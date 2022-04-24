// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ConversationEntry : MonoBehaviour
// {
//     public List<Person> targetPersonList = new List<Person>();
//     List<ActionPointHandler> APHList = new List<ActionPointHandler>();
//     // List<ActionPoint> firstAPs = new List<ActionPoint>();
//     //    List<float> firstDuringTime = new List<float>();
//     public bool isStartingByForce;
//     public bool isTalkingByVoice;
//     public bool isRandomTarget;
//     public int targetIncount = 0;
//     public bool isStartConversation = false;
//     private void Awake()
//     {
//         var APGroup = transform.Find("APGroup");
//         for (int i = 0; i < APGroup.childCount; i++)
//         {
//             APHList.Add(APGroup.GetChild(i).GetComponent<ActionPointHandler>());
//         }

//     }

//     // private void Start()
//     // {
//     //     APHList.ForEach(x => x.WaitForStartToNext(x.actionPoints.Count - 1, true));
//     // }
//     private void OnTriggerEnter(Collider other)
//     {
//         if (isRandomTarget)
//         {

//         }
//         else
//         {
//             CheckFixedTarget(other);
//         }

//         if (!isStartConversation
//             && targetIncount == targetPersonList.Count)
//         {
//             isStartConversation = true;
//             StartCoroutine(WaitForStandingOnStartPosition());
//         }
//     }

//     void CheckFixedTarget(Collider other)
//     {
//         if (other.CompareTag("PersonModel"))
//         {
//             for (int i = 0; i < targetPersonList.Count; i++)
//             {
//                 var nowPersonName = targetPersonList[i].name;
//                 if (other.transform.parent.gameObject.name == nowPersonName)
//                 {
//                     targetIncount++;
//                     SetConversationBuild(other.transform.parent.GetComponent<Person>());
//                     break;
//                 }
//             }
//         }
//     }

//     void SetConversationBuild(Person target)
//     {
//         for (int i = 0; i < APHList.Count; i++)
//         {
//             var nowGroup = APHList[i];
//             if (target.gameObject.name == nowGroup.gameObject.name)
//             {
//                 nowGroup.WaitForStartToNext(0, true);
//                 target.ChangeAPHandler(nowGroup);
//                 target.conversationEntry = this;
//             }
//         }
//     }

//     IEnumerator WaitForStandingOnStartPosition()
//     {
//         var canPass = false;
//         while (!canPass)
//         {
//             canPass = true;
//             for (int i = 0; i < targetPersonList.Count; i++)
//             {
//                 var targetP = targetPersonList[i];
//                 if (!targetP.IsStandingOnPosition(targetP.actionPointHandler.GetActionPoint(0).transform.position))
//                 {
//                     canPass = false;
//                     break;
//                 }
//             }
//             yield return new WaitForFixedUpdate();
//         }

//         yield return StartCoroutine(GivingSomeTimeForPlayAPAnimation());

//         StartTalk();
//         yield return null;
//     }
//     void StartTalk()
//     {
//         for (int i = 0; i < APHList.Count; i++)
//         {
//             APHList[i].WaitForStartToNext(0, false);
//         }
//         StartCoroutine(DoStartTalk());
//     }

//     IEnumerator DoStartTalk()
//     {
//         var canPass = false;
//         do
//         {
//             var nowPerson = targetPersonList[0];
//             targetPersonList.Remove(nowPerson);

//             nowPerson.model.ShowThreeD_Icon(PersonModel.ThreeD_IconList.SpeechBubble);
//             yield return new WaitForSeconds(Random.Range(1, 5));

//             if (targetPersonList.Count > 1)
//                 targetPersonList.Insert(Random.Range(1, targetPersonList.Count), nowPerson);
//             else
//                 targetPersonList.Add(nowPerson);

//             nowPerson.model.HideAllThreeD_Icon();

//             //checking for standing on end of AP
//             canPass = true;
//             for (int i = 0; i < APHList.Count; i++)
//             {
//                 var nowAPH = APHList[i];
//                 if (!nowAPH.IsReachedToEnd)
//                 {
//                     canPass = false;
//                     break;
//                 }
//             }

//             if (canPass)
//             {
//                 for (int i = 0; i < targetPersonList.Count; i++)
//                 {
//                     var p = targetPersonList[i];
//                     if (p.BeforeAlertLevel == Person.AlertLevel.Notice)
//                     {
//                         canPass = true;
//                         break;
//                     }
//                     else if (!p.IsStandingOnPosition(p.actionPointHandler.GetEndActionPoint.transform.position))
//                     {
//                         canPass = false;
//                         break;
//                     }
//                 }
//             }
//         } while (!canPass);

//         yield return StartCoroutine(GivingSomeTimeForPlayAPAnimation());

//         MakeReset();
//         yield return null;
//     }

//     void MakeReset()
//     {
//         for (int i = 0; i < targetPersonList.Count; i++)
//         {
//             var nowTarget = targetPersonList[i];
//             nowTarget.ChangeAPHandler(null);
//         }

//         for (int i = 0; i < APHList.Count; i++)
//         {
//             APHList[i].IsReachedToEnd = false;
//             APHList[i].ResetIndex();
//         }

//         isStartConversation = false;
//         targetIncount = 0;
//     }

//     IEnumerator GivingSomeTimeForPlayAPAnimation()
//     {
//         yield return new WaitForSeconds(1f);
//     }
// }
