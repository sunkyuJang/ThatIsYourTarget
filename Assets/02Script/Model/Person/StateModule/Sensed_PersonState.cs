using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sensed_PersonState : PersonState
{
    public List<SensedPrepareData> sensedPrepareDatas { private set; get; } = new List<SensedPrepareData>();
    Coroutine trackingBySensedPrepareData = null;
    readonly public List<StateKinds> PriolityList =
        new List<StateKinds>()
        {
            StateKinds.Curiousity,
            StateKinds.Patrol,
            StateKinds.Tracking,
            StateKinds.DrawWeapon,
            StateKinds.Hit,
        };
    public Sensed_PersonState(Person person) : base(person) { }
    public override bool IsReady()
    {
        return true;
    }

    protected override void StartModule()
    {
        var sensedData = prepareData as SensedPrepareData;
        var findData = sensedPrepareDatas.Find(x => x.target == sensedData.target);
        if (findData == null)
        {
            if (sensedData.isInSight)
                sensedPrepareDatas.Add(sensedData);
        }
        else
        {
            findData.isInSight = sensedData.isInSight;
        }

        if (trackingBySensedPrepareData == null)
            trackingBySensedPrepareData = StartCoroutine(DoSelectTrackingModel());
    }
    IEnumerator DoSelectTrackingModel()
    {
        var trackingList = new List<Transform>();
        while (sensedPrepareDatas.Any())
        {
            for (int i = 0; i < sensedPrepareDatas.Count; i++)
            {
                var prepareData = sensedPrepareDatas[i];
                if (prepareData.isInSight)
                {
                    var canSeeTarget = IsInSight(prepareData.target);
                    if (canSeeTarget)
                    {
                        trackingList.Add(prepareData.target);
                    }
                }
            }

            if (trackingList.Any())
            {
                var playingTargetModel = ModuleHandler.GetPlayingModuleTarget(StateKinds.Sensed);
                var selectedModel = SelectModel(trackingList, playingTargetModel);
                if (selectedModel != playingTargetModel)
                {
                    var dist = Vector3.Distance(ActorTransform.transform.position, selectedModel.position);
                    var shouldAttack = dist < DrawWeapon_PersonState.AbsoluteAttackDist;
                    SetState(shouldAttack ? StateKinds.DrawWeapon : StateKinds.Curiousity, new PersonPrepareData(selectedModel));
                    var selectedModelIndex = sensedPrepareDatas.FindIndex(x => x.target == selectedModel);
                    sensedPrepareDatas.RemoveAt(selectedModelIndex);
                }

                trackingList.Clear();
            }

            sensedPrepareDatas.RemoveAll(x => !x.isInSight);
            yield return new WaitForFixedUpdate();
        }

        trackingBySensedPrepareData = null;
        yield break;
    }

    Transform SelectModel(List<Transform> targets, Transform playingTarget)
    {
        List<(Transform target, int priolity, float dist)> eachModelPriolity = new List<(Transform target, int priolity, float dist)>();
        var playingModuleHandler = ModuleHandler;
        if (playingTarget != null)
        {
            var isContained = PriolityList.Contains(playingModuleHandler.GetPlayingModuleStateKind());
            var playingPriolity = isContained ? PriolityList.FindIndex(x => x == playingModuleHandler.GetPlayingModuleStateKind()) : 0;
            eachModelPriolity.Add(GetPriolityTuple(playingTarget, playingPriolity));
        }

        targets.ForEach(x =>
        {
            eachModelPriolity.Add(GetPriolityTuple(x));
        });

        eachModelPriolity.Sort((x, y) => y.priolity.CompareTo(x.priolity));
        var highestPriolity = eachModelPriolity.First().priolity;
        eachModelPriolity.RemoveAll(x => x.priolity < highestPriolity);
        eachModelPriolity.Sort((x, y) => x.dist.CompareTo(y.dist));

        return eachModelPriolity.First().target;
    }

    (Transform model, int priolity, float dist) GetPriolityTuple(Transform target, int priolity = 0)
    {
        var dist = Vector3.Distance(ActorTransform.position, target.position);
        var shouldAttack = dist <= DrawWeapon_PersonState.AbsoluteAttackDist;
        priolity += priolity == 0 ?
                        PriolityList.IndexOf(shouldAttack ? StateKinds.DrawWeapon : StateKinds.Curiousity) :
                        priolity;

        priolity += Person.GetPriolity(target);

        return (target, priolity, dist);
    }

    public override void EnterToException()
    {

    }
    public class SensedPrepareData : PersonPrepareData
    {
        public bool isInSight = false;
        public SensedPrepareData(Transform target, bool isInSight) : base(target)
        {
            this.isInSight = isInSight;
        }
    }
}
