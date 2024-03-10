using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;
public class RequirementConnector_Animator : StateMachineBehaviour
{
    public int trantisionGoTo = 0;
    public RequirementDataManager requirementDataManager;
}


[CustomEditor(typeof(RequirementConnector_Animator))]
public class RequirementConnector_AnimatorEditor : Editor
{
    public bool foldDetail = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Access the target object (RequirementConnector_Animator)
        RequirementConnector_Animator connector = (RequirementConnector_Animator)target;

        // 새 RequirementData를 추가하는 버튼을 만듭니다.
        if (GUILayout.Button("Add New RequirementData"))
        {
            // 새 RequirementData 인스턴스를 생성합니다.
            SkillRequirementData newRequirementData = CreateNewRequirementData();
            if (newRequirementData != null)
            {
                // 생성된 RequirementData 인스턴스를 requirementDatas 리스트에 추가합니다.
                connector.requirementDataManager.RequirementDatas.Add(newRequirementData);

                // 변경사항을 적용합니다.
                EditorUtility.SetDirty(connector);
                EditorUtility.SetDirty(newRequirementData);
            }
        }

        foldDetail = EditorGUILayout.Foldout(foldDetail, "필수요소 표시");
        if (foldDetail)
        {
            if (connector.requirementDataManager.RequirementDatas != null)
            {
                var index = 0;
                connector.requirementDataManager.RequirementDatas.ForEach(x =>
                {
                    if (x != null)
                    {
                        // Draw RequirementData
                        EditorGUILayout.LabelField(index++.ToString() + "번쨰 필수요소", EditorStyles.boldLabel);
                        SerializedObject serializedRequirementData = new SerializedObject(x);
                        RequirementDataDrawer.DrawRequirementData(x);
                        EditorGUILayout.LabelField("", EditorStyles.boldLabel);
                    }
                });
            }
        }

        EditorGUILayout.LabelField("", EditorStyles.boldLabel);
    }

    private SkillRequirementData CreateNewRequirementData()
    {
        // 새 RequirementData ScriptableObject를 생성합니다.
        SkillRequirementData newRequirementData = ScriptableObject.CreateInstance<SkillRequirementData>();

        string path = "Assets/";
        string requirementName = "newRequirement" + ".asset";

        // 적절한 위치에 새 ScriptableObject를 저장합니다.
        string definePath = AssetDatabase.GenerateUniqueAssetPath(path + requirementName);
        AssetDatabase.CreateAsset(newRequirementData, definePath);
        AssetDatabase.SaveAssets();

        return newRequirementData;
    }
}
