using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ApplyTmpFont))]
public class ApplyTmpFontEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        var tool = (ApplyTmpFont)target;

        using (new EditorGUI.DisabledScope(tool.fontAsset == null))
        {
            if (GUILayout.Button("应用到自身及所有子物体", GUILayout.Height(28)))
            {
                var texts = tool.GetComponentsInChildren<TMP_Text>(true);
                Undo.RecordObjects(texts, "Apply TMP Font");

                tool.ApplyFont();

                EditorUtility.SetDirty(tool);
                for (int i = 0; i < texts.Length; i++)
                    EditorUtility.SetDirty(texts[i]);
            }
        }
    }
}
