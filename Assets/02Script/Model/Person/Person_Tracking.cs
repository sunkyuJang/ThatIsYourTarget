// using System.Linq;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using JMath;


// public partial class Person : Model
// {
//     private List<CheckingTrackingState> CheckingTrackingStates { set; get; } = new List<CheckingTrackingState>();
//     private Coroutine procDoCheckingPlayers = null;
//     private Coroutine procDoTrackingClosePlayer = null;
//     public override void Contected(Collider collider)
//     {

//     }

//     public override void Removed(Collider collider)
//     {
//         switch (collider.tag)
//         {
//             case Player.playerTag:
//                 RemoveFromTrackingPlayerState(collider.transform);
//                 break;
//         }
//     }

//     void RemoveFromTrackingPlayerState(Transform target)
//     {
//         //it will just reservate for remove.
//         if (CheckingTrackingStates.Count > 0)
//         {
//             var find = CheckingTrackingStates.Find(x => x.target == target);
//             if (find != null)
//             {
//                 find.shouldRemove = true;
//             }
//         }
//     }

//     void AddToTrackingTargetState(Transform target)
//     {
//         CheckingTrackingState checkingPlayerState = null;
//         var find = CheckingTrackingStates.FirstOrDefault(x => x.target == target);
//         if (find == null)
//             checkingPlayerState = new CheckingTrackingState(target);

//         if (checkingPlayerState != null)
//         {
//             CheckingTrackingStates.Add(checkingPlayerState);
//             CheckPlayersState();
//         }
//     }

//     void CheckPlayersState()
//     {
//         if (procDoCheckingPlayers == null)
//             procDoCheckingPlayers = StartCoroutine(DoCheckPlayersState());
//     }

//     IEnumerator DoCheckPlayersState()
//     {   //find most close one in the list
//         CheckingTrackingState mostCloseState = null;
//         CheckingTrackingState lastMostCloseState = null;
//         while (CheckingTrackingStates.Count != 0)
//         {
//             mostCloseState = GetMostClosedOne(mostCloseState);

//             if (mostCloseState != lastMostCloseState)
//             {
//                 lastMostCloseState = mostCloseState;
//                 StartClosePlayerTracking(mostCloseState);
//             }
//             yield return new WaitForFixedUpdate();
//         }

//         procDoCheckingPlayers = null;
//         yield return null;
//     }

//     CheckingTrackingState GetMostClosedOne(CheckingTrackingState mostCloseState = null)
//     {
//         for (int i = 0; i < CheckingTrackingStates.Count; i++)
//         {
//             var data = CheckingTrackingStates[i];
//             if (data.CanRemove)
//             {
//                 CheckingTrackingStates.RemoveAt(i--);
//             }
//             else
//             {
//                 if (!data.isFollowing &&
//                     ShouldRecongnize(data.target))
//                 {
//                     if (mostCloseState == null)
//                     {
//                         mostCloseState = data;
//                     }
//                     else
//                     {
//                         Transform[] list = new Transform[] { mostCloseState.target, data.target };
//                         var closedTransform = Vector3Extentioner.GetMostClosedOne(modelHandler.transform, list);
//                         var shouldCloseOneChage = data.target == closedTransform;
//                         mostCloseState = shouldCloseOneChage ? data : mostCloseState;
//                     }
//                 }
//             }
//         }

//         return mostCloseState;
//     }

//     void StartClosePlayerTracking(CheckingTrackingState data)
//     {
//         if (procDoTrackingClosePlayer != null)
//             StopCoroutine(procDoTrackingClosePlayer);

//         procDoTrackingClosePlayer = StartCoroutine(DoClosedPlayerTracking(data));
//     }
//     IEnumerator DoClosedPlayerTracking(CheckingTrackingState data)
//     {
//         CheckingTrackingStates.ForEach(x => x.isFollowing = x == data);

//         var beforeState = (PersonState.StateKinds)state;
//         var lastPlayerPosition = data.target.position;
//         var lastAPH = modelHandler.actionPointHandler;
//         while (!data.shouldRemove)
//         {
//             var shouldResetTrackingPosition = lastPlayerPosition != data.target.position;
//             var nowState = GetStateByDist(data.target.position);
//             if (nowState != beforeState)
//             {
//                 beforeState = nowState;
//                 ActionPointHandler aph = GetEachStateOfAPH(nowState, data.target);
//                 SetAPH(aph);
//                 lastAPH = modelHandler.actionPointHandler;
//                 SetState((int)beforeState);
//             }

//             if (shouldResetTrackingPosition)
//             {
//                 modelHandler.ChageLastAPPosition(data.target);
//                 lastPlayerPosition = data.target.position;
//             }

//             yield return new WaitForFixedUpdate();
//         }

//         SetState((int)PersonState.StateKinds.Normal);
//         data.isFollowing = false;
//         procDoTrackingClosePlayer = null;
//     }

//     ActionPointHandler GetEachStateOfAPH(PersonState.StateKinds kinds, Transform playerTranform)
//     {
//         ActionPointHandler aph = null;
//         switch (kinds)
//         {
//             case PersonState.StateKinds.Curiousity:
//                 aph = GetNoticeAPH(playerTranform);
//                 break;
//         }

//         return aph;
//     }

//     PersonState.StateKinds GetStateByDist(Vector3 WPosition)
//     {
//         var dist = Vector3.Distance(modelHandler.transform.position, WPosition);
//         return PersonState.StateKinds.Curiousity;
//     }

//     ActionPointHandler GetNoticeAPH(Transform playerTranform)
//     {
//         var requireAPCount = 2;
//         var apPooler = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP);
//         var APs = new List<ActionPoint>();
//         APs.Capacity = requireAPCount;

//         for (int i = 0; i < requireAPCount; i++)
//             APs.Add(apPooler.GetNewOne<PersonActionPoint>());

//         var aph = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.APH).GetNewOne<ActionPointHandler>();
//         aph.SetAPs(APs);
//         aph.ShouldLoop = false;

//         SetAPWithFixedDuring(APs[0], playerTranform, PersonAniController.StateKind.Surprize);
//         SetAPWithFixedDuring(APs[1], playerTranform, PersonAniController.StateKind.LookAround, true, true);

//         return aph;
//     }

//     void SetAPWithFixedDuring(ActionPoint ap, Transform target, PersonAniController.StateKind kind, bool shouldChangePosition = false, bool shouldChangeRotation = false)
//     {
//         ap.SetAPWithFixedDuring(modelHandler.transform, target, (int)kind, kind.ToString(), shouldChangePosition, shouldChangeRotation);
//     }
// }
