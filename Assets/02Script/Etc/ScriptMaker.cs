using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptMaker
{
    public static string folderPath = "Assets/02Script/AnimatorReader";
    // public static GetScript(string name)
    // {
    //     if (File.Exists(GetFullPath(name)))
    //     {

    //     }
    //     else
    //     {
    //         File.WriteAllText()
    //     }
    // }

    public static string GetFullPath(string name) => folderPath + "/" + name + ".txt";
}
