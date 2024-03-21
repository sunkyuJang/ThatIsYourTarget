using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using Unity.XR.CoreUtils;

public class SkillConnector_Animator : StateMachineBehaviour
{
    public string SkillName = "";
    public SkillData skillData;
}

[CustomEditor(typeof(SkillConnector_Animator))]
public class SkillConnector_AnimatorEditor : Editor
{
    public bool foldDetail = true;
    private SkillDataEditor skillDataEditor;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Access the target object (SkillConnector_Animator)
        SkillConnector_Animator connector = (SkillConnector_Animator)target;

        if (connector.skillData == null)
        {
            var redTextStyle = new GUIStyle(GUI.skin.label);
            redTextStyle.normal.textColor = Color.red;
            redTextStyle.fontStyle = FontStyle.Bold;
            redTextStyle.alignment = TextAnchor.MiddleCenter;
            if (GUILayout.Button("새로운 스킬데이터 추가"))
            {
                //var skillKeyName = GetCurrentStateMachineName(connector);
                connector.skillData = CreateNewSkillData(connector.SkillName);
            }
            EditorGUILayout.LabelField("*반드시 KeyName은 스테이트와 같은 이름으로 할 것.", redTextStyle);
            return;
        }

        if (skillDataEditor == null)
        {
            skillDataEditor = (SkillDataEditor)CreateEditor(connector.skillData, typeof(SkillDataEditor));
        }
        skillDataEditor?.OnInspectorGUI();
    }

    private SkillData CreateNewSkillData(string skillName)
    {
        // 새 RequirementData ScriptableObject를 생성합니다.
        SkillData skillData = ScriptableObject.CreateInstance<SkillData>();

        string path = "Assets/";
        string requirementName = skillName + "_SkillData" + ".asset";

        // 적절한 위치에 새 ScriptableObject를 저장합니다.
        string definePath = AssetDatabase.GenerateUniqueAssetPath(path + requirementName);
        AssetDatabase.CreateAsset(skillData, definePath);
        AssetDatabase.SaveAssets();

        return skillData;
    }

    // private string GetCurrentStateMachineName(SkillConnector_Animator connector)
    // {
    //     // AnimatorController와 AnimatorStateMachine에 대한 참조를 가져옵니다.
    //     AnimatorController controller = connector.GetComponent<Animator>()?.runtimeAnimatorController as AnimatorController;
    //     if (controller == null) return null;

    //     // AnimatorController에서 모든 layers를 순회하며 현재 SkillConnector_Animator가 속한 state를 찾습니다.
    //     foreach (var layer in controller.layers)
    //     {
    //         foreach (var state in layer.stateMachine.states)
    //         {
    //             if (state.state.behaviours.Contains(connector))
    //             {
    //                 // 현재 state가 속한 state machine의 이름을 반환합니다.
    //                 return layer.stateMachine.name;
    //             }
    //         }
    //     }

    //     return null; // 스테이트머신을 찾지 못했다면 null 반환
    // }

}
