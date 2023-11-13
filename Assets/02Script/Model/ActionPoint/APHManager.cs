using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class APHManager : MonoBehaviour
{
    public static APHManager Instance { set; get; }
    public bool IsReady { private set; get; } = false;
    [SerializeField]
    public enum PoolerKinds { APH = 0, PersonAP }
    public List<GameObject> poolerPrefab = new List<GameObject>();
    Dictionary<Type, ObjPooler> APDictionary { set; get; } = new Dictionary<Type, ObjPooler>();

    private ObjPooler GetObjPooler<T>()
    {
        if (APDictionary.ContainsKey(typeof(T)))
        {
            return APDictionary[typeof(T)];
        }

        return null;
    }

    private ObjPooler GetAPPooler(GameObject gameObject)
    {
        var componetType = GetPrefabType(gameObject);
        if (APDictionary.ContainsKey(componetType))
            return APDictionary[componetType];
        else
            Debug.Log("something wrong with ap pooler");

        return null;
    }

    private ObjPooler GetAPPooler<T>() where T : AnimationPoint
    {
        if (APDictionary.ContainsKey(typeof(T)))
            return GetObjPooler<T>();

        return null;
    }

    public AnimationPointHandler GetNewAPH()
    {
        if (APDictionary.ContainsKey(typeof(AnimationPointHandler)))
        {
            return APDictionary[typeof(AnimationPointHandler)].GetNewOne<AnimationPointHandler>();
        }

        return null;
    }

    public T GetNewAP<T>() where T : AnimationPoint
    {
        var APPooler = GetAPPooler<T>();
        if (APPooler != null) return APPooler.GetNewOne<T>();

        return null;
    }
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
        poolerPrefab.ForEach(poolerPrefab =>
        {
            var componetType = GetPrefabType(poolerPrefab);
            if (componetType != null)
            {
                APDictionary.Add(componetType, ObjPoolerManager.Instance.GetPooler(poolerPrefab));
                for (int i = 20; i > 0; i--)
                    APDictionary[componetType].MakeNewOne();
            }
        });
        IsReady = true;
    }

    Type GetPrefabType(GameObject poolerPrefab)
    {
        var components = poolerPrefab.GetComponents<Component>();
        foreach (var poolerComponent in components)
        {
            var itemName = poolerComponent.GetType().Name;
            if (itemName != "Transform")
            {
                return poolerComponent.GetType();
            }
        }

        return null;
    }

    public AnimationPointHandler GetCopyAPH<T>(AnimationPointHandler originalAPH) where T : AnimationPoint
    {
        var aph = GetCoiedAPH(originalAPH);
        var copiedAPs = GetCopiedAPs<T>(aph, originalAPH.animationPoints);
        aph.animationPoints = copiedAPs;

        return aph;
    }

    AnimationPointHandler GetCoiedAPH(AnimationPointHandler originalAPH)
    {
        var coiedAPH = GetNewAPH();
        ObjPooler.CopyComponentValue(originalAPH.transform, coiedAPH.transform);
        return coiedAPH;
    }

    List<AnimationPoint> GetCopiedAPs<T>(AnimationPointHandler copiedAPH, List<AnimationPoint> originalAPs) where T : AnimationPoint
    {
        var copiedAPs = new List<AnimationPoint>();

        for (int i = 0; i < originalAPs.Count; i++)
        {
            var originalAP = originalAPs[i];
            var ap = GetNewAP<T>();
            ap.transform.SetParent(copiedAPH.transform);

            if (originalAP is T)
            {
                ObjPooler.CopyComponentValue(originalAP, ap);
            }
            else
            {
                ObjPooler.CopyComponentValue(originalAP.transform, ap.transform);
            }

            copiedAPs.Add(ap);
        }

        return copiedAPs;
    }

    public AnimationPointHandler GetNewAPH<T>(Transform APHGroup, int APCounts, AnimationPointHandler.WalkingState walkingState = AnimationPointHandler.WalkingState.Walk) where T : AnimationPoint
    {
        var apPooler = GetObjPooler<T>();
        if (apPooler == null) return null;

        var requireAPCount = APCounts;
        var APs = new List<T>()
        {
            Capacity = requireAPCount
        };

        for (int i = 0; i < requireAPCount; i++)
        {
            var ap = apPooler.GetNewOne<T>();
            ap.gameObject.SetActive(true);
            APs.Add(ap);
        }

        var aph = GetNewAPH();
        aph.transform.SetParent(APHGroup);
        aph.gameObject.SetActive(true);
        aph.SetAPs(APs);
        aph.shouldLoop = false;
        aph.walkingState = walkingState;
        return aph;
    }

    public void ReturnAPH(AnimationPointHandler handler)
    {
        handler.animationPoints.ForEach(x =>
        {
            var pooler = GetAPPooler(x.gameObject);
            pooler?.ReturnTargetObj(x.gameObject);
        });

        GetObjPooler<AnimationPointHandler>().ReturnTargetObj(handler.gameObject);
    }

    public void ReturnAP(GameObject gameObject)
    {
        var pooler = GetAPPooler(gameObject);
        pooler?.ReturnTargetObj(gameObject);
    }
}
