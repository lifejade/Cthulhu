using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static RegionNode;

[CustomEditor(typeof(RegionNode))]
public class RegionNodeInspectEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        // _objectType 필드를 가져옵니다.
        var objectType = (Board_State)serializedObject.FindProperty("_board_state").intValue;
        // _objectType 필드를 Inspector에 노출 시켜줍니다.
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_board_state"));
        
        // Inspector에서 ObjectType Enum을 변경하게 되면 해당 타입에 맞게 노출시켜줄 필드를 정의해줍니다.
        switch (objectType)
        {
            case Board_State.heroine:
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("SceneName"));
                }
                break;
            case Board_State.enter:
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("SceneName"));
                }
                break;
        }


        // 변경된 프로퍼티를 저장해줍니다.
        serializedObject.ApplyModifiedProperties();
    }
}
