using TMPro;
using UnityEngine;

public class HumanInfoUI : MonoBehaviour
{
    public Human human;
    public TextMeshProUGUI StateModule;
    private void Update()
    {
        if (human != null)
        {
            //StateModule.text = ((PersonState.StateKinds)person.moduleHandler.GetPlayingModuleIndex()).ToString();
        }
    }
}
