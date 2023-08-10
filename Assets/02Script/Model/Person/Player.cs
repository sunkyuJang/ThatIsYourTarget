using System.Collections;
using UnityEngine;

public class Player : Model
{
    public const string playerTag = "Player";
    [SerializeField]
    private Renderer renderer;
    public Material belongTo { get { return renderer.material; } }

    protected override StateModuleHandler SetStateModuleHandler()
    {
        throw new System.NotImplementedException();
    }

    protected override void Awake()
    {
        modelHandler = GetComponentInChildren<ModelHandler>();
        return;
    }

    protected override IEnumerator Start()
    {
        yield break;
    }
}
