using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationWindow : MonoBehaviour
{
    public Text leftTargetText;

    private void Update()
    {
        if (TargetManager.IsExist)
        {
            leftTargetText.text = "남은 타겟 : " + TargetManager.instance.GetLeftTarget().ToString();
        }
    }
}
