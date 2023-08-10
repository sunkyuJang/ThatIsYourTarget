using TMPro;
using UnityEngine;

public class PersonInfoUI : MonoBehaviour
{
    public Person person;
    public TextMeshProUGUI StateModule;
    private void Update()
    {
        if (person != null)
        {
            //StateModule.text = ((PersonState.StateKinds)person.moduleHandler.GetPlayingModuleIndex()).ToString();
        }
    }
}
