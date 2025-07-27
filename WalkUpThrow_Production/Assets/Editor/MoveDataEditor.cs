using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using FightinGameEngine;

[CustomEditor(typeof(MoveData))]
public class MoveDataEditor : Editor
{
    private MoveData moveData;

    private void OnEnable()
    {
        moveData = (MoveData)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        moveData.moveName = EditorGUILayout.TextField("Move Name", moveData.moveName);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Hitbox Frames", EditorStyles.boldLabel);

        for (int i = 0; i < moveData.hitboxFrames.Count; i++)
        {
            var frame = moveData.hitboxFrames[i];

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            frame.frame = EditorGUILayout.IntField("Frame", frame.frame);
            if (GUILayout.Button("Remove Frame", GUILayout.Width(120)))
            {
                moveData.hitboxFrames.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();

            for (int j = 0; j < frame.hitboxes.Count; j++)
            {
                var hitbox = frame.hitboxes[j];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Hitbox {j}", EditorStyles.boldLabel);
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    frame.hitboxes.RemoveAt(j);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                hitbox.type = (HitboxType)EditorGUILayout.EnumPopup("Type", hitbox.type);
                hitbox.box = EditorGUILayout.RectField("Box", hitbox.box);
                EditorGUILayout.Space();
            }

            if (GUILayout.Button("Add Hitbox"))
            {
                frame.hitboxes.Add(new HitboxData());
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Frame"))
        {
            moveData.hitboxFrames.Add(new HitboxFrame());
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(moveData);
        }
    }
}
