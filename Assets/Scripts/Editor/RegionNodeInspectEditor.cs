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

        // _objectType �ʵ带 �����ɴϴ�.
        var objectType = (Board_State)serializedObject.FindProperty("_board_state").intValue;
        // _objectType �ʵ带 Inspector�� ���� �����ݴϴ�.
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_board_state"));
        
        // Inspector���� ObjectType Enum�� �����ϰ� �Ǹ� �ش� Ÿ�Կ� �°� ��������� �ʵ带 �������ݴϴ�.
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


        // ����� ������Ƽ�� �������ݴϴ�.
        serializedObject.ApplyModifiedProperties();
    }
}
