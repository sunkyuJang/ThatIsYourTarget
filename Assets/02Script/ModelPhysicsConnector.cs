using UnityEngine;

public class ModelPhysicsConnector : MonoBehaviour
{
    Model Model { set; get; }
    private void Awake()
    {
        Model = transform.parent.GetComponent<Model>();
    }
}
