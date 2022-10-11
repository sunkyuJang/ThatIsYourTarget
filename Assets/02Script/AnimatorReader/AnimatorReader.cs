using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnimatorReader : MonoBehaviour
{
    [SerializeField] internal List<Animator> animators;
    private StreamWriter FoamToNewBasicScript;
}

[CustomEditor(typeof(AnimatorReader))]
public class AnimatorReaderEditor : Editor
{
    private void OnInspectorUpdate()
    {
        var AnimatorReader = (AnimatorReader)target;
    }

    private void CreatScript(AnimatorReader reader)
    {
        foreach (var animator in reader.animators)
        {

        }
    }
}


