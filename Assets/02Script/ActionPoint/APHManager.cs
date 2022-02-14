using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APHManager : MonoBehaviour
{
    public static APHManager Instance { set; get; }
    public GameObject APHPrefab;
    public GameObject APPrefab;
    public ObjPooler APHPooler { set; get; }
    public ObjPooler APPooler { set; get; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        APHPooler = ObjPoolerManager.Instance.GetPooler(APHPrefab);
        for (int i = 0; i < 20; i++)
        {
            APHPooler.MakeNewOne();
        }

        APPooler = ObjPoolerManager.Instance.GetPooler(APPrefab);
        for (int i = 0; i < 20; i++)
        {
            APPooler.MakeNewOne();
        }
    }

    public ActionPointHandler GetAPHForNotice(Vector3 targetPosition, Vector3 positionFromRequester)
    {
        var ap = APPooler.GetNewOne<ActionPoint>();
        var aph = APHPooler.GetNewOne<ActionPointHandler>();

        ap.transform.position = targetPosition;
        ap.MakeLookAtTo(positionFromRequester, targetPosition);
        ap.state = ActionPoint.StateKind.lookAround;
        ap.during = 2f;
        ap.transform.SetParent(aph.transform);

        aph.ShouldLoop = false;
        aph.SetAPs();
        aph.comingFromOther = ReturnAPH;

        return aph;
    }

    public void ReturnAPH(ActionPointHandler handler)
    {
        handler.actionPoints.ForEach(x => APPooler.ReturnTargetObj(x.gameObject));
        APHPooler.ReturnTargetObj(handler.gameObject);
    }
}
