using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APHManager : MonoBehaviour
{
    public static APHManager Instance { set; get; }
    public bool IsReady { private set; get; } = false;
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
        IsReady = true;
    }

    public ActionPointHandler GetCopyAPH(ActionPointHandler originalAPH)
    {
        var aph = GetCoiedAPH(originalAPH);
        var copiedAPs = GetCopiedAPs(aph, originalAPH.actionPoints);
        aph.actionPoints = copiedAPs;

        return aph;
    }

    ActionPointHandler GetCoiedAPH(ActionPointHandler originalAPH)
    {
        var coiedAPH = APHPooler.GetNewOne<ActionPointHandler>();
        ObjPooler.CopyComponentValue(originalAPH.transform, coiedAPH.transform);
        //ObjPooler.CopyComponentValue(originalAPH, coiedAPH);
        return coiedAPH;
    }

    List<ActionPoint> GetCopiedAPs(ActionPointHandler copiedAPH, List<ActionPoint> originalAPs)
    {
        var copiedAPs = new List<ActionPoint>();

        for (int i = 0; i < originalAPs.Count; i++)
        {
            var originalAP = originalAPs[i];
            var apObj = APPooler.GetNewOne();
            apObj.transform.SetParent(copiedAPH.transform);

            ObjPooler.CopyComponentValue(originalAP.transform, apObj.transform);

            ActionPoint ap = null;
            if (originalAP is PersonActionPoint)
            {
                ap = apObj.AddComponent<PersonActionPoint>();
                ObjPooler.CopyComponentValue(originalAP as PersonActionPoint, ap as PersonActionPoint);
            }

            copiedAPs.Add(ap);
        }
        return copiedAPs;
    }

    public void ReturnAPH(ActionPointHandler handler)
    {
        handler.actionPoints.ForEach(x => APPooler.ReturnTargetObj(x.gameObject));
        APHPooler.ReturnTargetObj(handler.gameObject);
    }

    // public ActionPointHandler GetAPHForNotice(Vector3 targetPosition, Vector3 positionFromRequester)
    // {
    //     var ap = APPooler.GetNewOne<ActionPoint>();
    //     var aph = APHPooler.GetNewOne<ActionPointHandler>();

    //     ap.transform.position = targetPosition;
    //     ap.MakeLookAtTo(positionFromRequester, targetPosition);
    //     ap.state = ActionPoint.StateKind.lookAround;
    //     ap.during = 2f;
    //     ap.transform.SetParent(aph.transform);

    //     aph.ShouldLoop = false;
    //     aph.SetAPs();
    //     aph.comingFromOther = ReturnAPH;

    //     return aph;
    // }
}
