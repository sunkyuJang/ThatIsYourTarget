using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class Person : Model
{
    private PersonAniController aniController;
    public enum StateKinds { Normal, Notice, Warn, Follow, Wait, Attack, Avoid, Dead, Non }
    enum StateByDist { Notice = 3, Attack = 1 }
    [SerializeField]
    private Renderer modelRenderer;

    protected override IEnumerator Start()
    {
        yield return StartCoroutine(base.Start());
        aniController = modelHandler.aniController as PersonAniController;

        yield return null;
    }
    public Material belongTo
    {
        set { modelRenderer.material = value; }
        get { return modelRenderer.material; }
    }

    bool ShouldRecongnize(Player player) => player.belongTo == belongTo;

    public override void ChangedState(int state)
    {
        switch ((StateKinds)state)
        {
            case StateKinds.Normal: SetOriginalAPH(); break;
            default: break;
        }
    }

    public override void GetHit()
    {

    }
}
