using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

public class Player : Model
{
    public const string playerTag = "Player";
    [SerializeField]
    private Renderer renderer;
    public Material belongTo { get { return renderer.material; } }

    new private void Awake()
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

    protected override SkillLoader SetSkillLoader(AnimatorController controller)
    {
        throw new System.NotImplementedException();
    }

    public override void OnDetected(Collider collider)
    {
        throw new System.NotImplementedException();
    }
}
