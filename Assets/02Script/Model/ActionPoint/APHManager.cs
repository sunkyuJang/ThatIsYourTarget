using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APHManager : MonoBehaviour
{
    public static APHManager Instance { set; get; }
    public bool IsReady { private set; get; } = false;
    [SerializeField]
    public enum PoolerKinds { APH = 0, PersonAP }
    public List<GameObject> poolerPrefab = new List<GameObject>();
    List<ObjPooler> poolers = new List<ObjPooler>();
    public ObjPooler GetObjPooler(PoolerKinds kinds) => poolers[(int)kinds];
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
        poolers.Add(ObjPoolerManager.Instance.GetPooler(poolerPrefab[(int)PoolerKinds.APH]));
        for (int i = 0; i < 20; i++)
        {
            poolers[(int)PoolerKinds.APH].MakeNewOne();
        }

        poolers.Add(ObjPoolerManager.Instance.GetPooler(poolerPrefab[(int)PoolerKinds.PersonAP]));
        for (int i = 0; i < 20; i++)
        {
            poolers[(int)PoolerKinds.PersonAP].MakeNewOne();
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
        var coiedAPH = GetObjPooler(PoolerKinds.APH).GetNewOne<ActionPointHandler>();
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
            var ap = GetObjPooler(PoolerKinds.PersonAP).GetNewOne<ActionPoint>();
            ap.transform.SetParent(copiedAPH.transform);

            ObjPooler.CopyComponentValue(originalAP.transform, ap.transform);

            if (originalAP is PersonActionPoint)
            {
                ObjPooler.CopyComponentValue(originalAP as PersonActionPoint, ap as PersonActionPoint);
            }

            copiedAPs.Add(ap);
        }
        return copiedAPs;
    }

    public void ReturnAPH(ActionPointHandler handler)
    {
        handler.actionPoints.ForEach(x => GetObjPooler(PoolerKinds.PersonAP).ReturnTargetObj(x.gameObject));
        GetObjPooler(PoolerKinds.APH).ReturnTargetObj(handler.gameObject);
    }
}
