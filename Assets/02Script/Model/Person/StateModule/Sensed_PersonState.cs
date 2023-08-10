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
            StateKinds.Curiousity, // 의심
            StateKinds.Patrol, // 놓쳤을 때
            StateKinds.Tracking, // 타겟을 놓치지 전까지
            StateKinds.PrepareAttack, // 발견 직후 공격 준비
            StateKinds.Hit, // 타겟이 힛범위 내에
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
            trackingBySensedPrepareData = person.StartCoroutine(DoSelectTrackingModel());
    }
    IEnumerator DoSelectTrackingModel()
    {
        var trackingList = new List<Model>();
        while (sensedPrepareDatas.Any())
        {
            for (int i = 0; i < sensedPrepareDatas.Count; i++)
            {
                var prepareData = sensedPrepareDatas[i];
                if (prepareData.isInSight)
                {
                    var canSeeTarget = person.modelHandler.IsInSight(prepareData.target.modelHandler.transform);
                    if (canSeeTarget)
                    {
                        trackingList.Add(prepareData.target);
                    }
                }
            }

            if (trackingList.Any())
            {
                var playingTargetModel = person.moduleHandler.GetPlayingModuleTarget(StateKinds.Sensed);
                var selectedModel = SelectModel(trackingList, playingTargetModel);
                if (selectedModel != playingTargetModel)
                {
                    var dist = Vector3.Distance(person.modelHandler.transform.position, selectedModel.modelHandler.transform.position);
                    var shouldAttack = dist < PrepareAttack_PersonState.prepareAttackDist;
                    SetState(shouldAttack ? StateKinds.PrepareAttack : StateKinds.Curiousity, new PersonPrepareData(selectedModel));
                    var selectedModelIndex = sensedPrepareDatas.FindIndex(x => x.target == selectedModel);
                    sensedPrepareDatas.RemoveAt(selectedModelIndex);
                }
            }

            sensedPrepareDatas.RemoveAll(x => !x.isInSight);
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }

    Model SelectModel(List<Model> targets, Model playingTarget)
    {
        List<(Model model, int priolity, float dist)> eachModelPriolity = new List<(Model model, int priolity, float dist)>();
        var playingModuleHandler = person.moduleHandler;
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

        return eachModelPriolity.First().model;
    }

    (Model model, int priolity, float dist) GetPriolityTuple(Model model, int priolity = 0)
    {
        var dist = Vector3.Distance(person.modelHandler.transform.position, model.modelHandler.transform.position);
        var shouldAttack = dist <= PrepareAttack_PersonState.prepareAttackDist;
        priolity += priolity == 0 ?
                        PriolityList.IndexOf(shouldAttack ? StateKinds.PrepareAttack : StateKinds.Curiousity) :
                        priolity;

        priolity += model as Player ? 5 : 0;

        return (model, priolity, dist);
    }

    public override void EnterToException()
    {

    }
    public class SensedPrepareData : PersonPrepareData
    {
        public bool isInSight = false;
        public SensedPrepareData(Model target, bool isInSight) : base(target)
        {
            this.isInSight = isInSight;
        }
    }
}
