using UnityEditor;
using UnityEngine;
using System.IO;

public class CodeAnalyzerWindow : EditorWindow
{
    private string directoryPath = "Your/Directory/Path";

    [MenuItem("Window/Code Analyzer")]
    public static void ShowWindow()
    {
        GetWindow<CodeAnalyzerWindow>("Code Analyzer");
    }

    void OnGUI()
    {
        GUILayout.Label("Code Analyzer", EditorStyles.boldLabel);

        if (GUILayout.Button("Analyze Code"))
        {
            AnalyzeCodeInDirectory(directoryPath);
        }
    }

    private void AnalyzeCodeInDirectory(string path)
    {
        var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var code = File.ReadAllText(file);
            //var tree = CSharpSyntaxTree.ParseText(code);
            // 여기에 분석 로직 추가
        }
    }
}