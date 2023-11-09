using System.Collections;
using UnityEngine;

public class Player : Model
{
    public const string playerTag = "Player";
    [SerializeField]
    private Renderer renderer;
    public Material belongTo { get { return renderer.material; } }

    private void Awake()
    {

    }
    protected override StateModuleHandler SetStateModuleHandler()
    {
        throw new System.NotImplementedException();
    }
    protected override ConversationHandler SetConversationHandler()
    {
        throw new System.NotImplementedException();
    }
    protected override IEnumerator Start()
    {
        yield break;
    }


}
