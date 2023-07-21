using UnityEngine;

public class Hit_PersonState : PersonState
{
    PrepareData prepareData { set; get; }
    public Hit_PersonState(Person person) : base(person) { }
    public void SetPrepareData(PrepareData prepareData)
    {
        this.prepareData = prepareData;
    }
    public override bool IsReadyForEnter()
    {
        return prepareData != null;
    }
    public override void EnterToException() { }
    protected override void DoEnter()
    {
        person.SetOriginalAPH();
    }
    public override void Exit() { }

    public class PrepareData
    {
        public Transform target { get; set; }
    }
}
