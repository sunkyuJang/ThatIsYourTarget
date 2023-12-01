using UnityEngine;

public class NaviTrafficManager : MonoBehaviour
{
    public static NaviTrafficManager Instance { private set; get; }
    private MetaphysicsTrafficHandler metaphysicsTrafficHandler;
    private PhysicsTrafficHandler physicsTrafficHandler;
    private float castRadius { get { return NaviController.eachStateDist[(int)NaviController.State.Close].Value; } }
    public int NaviAvoidance = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        metaphysicsTrafficHandler = new MetaphysicsTrafficHandler();
        physicsTrafficHandler = new PhysicsTrafficHandler();
        // 기타 초기화 로직
    }

    public bool IsCongested(Vector3 targetPosition, NaviController naviController, out MetaphysicsTrafficHandler.TrafficData trafficData)
    {
        return
            // MetaphysicsTrafficHandler를 통한 체크
            metaphysicsTrafficHandler.IsCongested(targetPosition, naviController, castRadius, out trafficData) ||
            // PhysicsTrafficHandler를 통한 체크
            physicsTrafficHandler.IsCongested(targetPosition, castRadius, naviController);
    }

    public void AddTrafficPointForPhysics(Vector3 position)
    {
        physicsTrafficHandler.IsCongested(position, castRadius, null);
    }

    public void AddTrafficPoint(MetaphysicsTrafficHandler.TrafficData trafficData)
    {
        metaphysicsTrafficHandler.AddData(trafficData);
    }
}