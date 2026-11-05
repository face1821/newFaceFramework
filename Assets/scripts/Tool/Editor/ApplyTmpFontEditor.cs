using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ApplyTmpFont))]  //告诉Unity，这个编辑器类负责绘制ApplyTmpFont组件的Inspector(面板)。
public class ApplyTmpFontEditor : Editor  //必须继承Editor类，并且此文件必须放在Editor文件夹下
{
    public override void OnInspectorGUI()  //重写OnInspectorGUI()，此方法用来绘制UI
    {
        DrawDefaultInspector();  //绘制组件默认布局

        EditorGUILayout.Space();  //增加一段垂直空白，用于排版
        var tool = (ApplyTmpFont)target;  //获取挂有脚本的物体实例

        using (new EditorGUI.DisabledScope(tool.fontAsset == null))  //设置了字体文件时按钮才亮起，否则无法按下
        {
            if (GUILayout.Button("应用到自身及所有子物体", GUILayout.Height(28)))  //点击按钮时执行if里的代码，并且设置按钮高度为28
            {
                var texts = tool.GetComponentsInChildren<TMP_Text>(true);  //获取游戏物体上的TMP_Text组件
                Undo.RecordObjects(texts, "Apply TMP Font");  //修改这些TMP_Text之前，先把它们当前状态记录到Unity的撤销系统，后续可按下Ctrl+Z撤销

                tool.ApplyFont();  //执行替换字体的函数

                EditorUtility.SetDirty(tool);  //把tool组件标记为"脏"(dirty)，告诉Unity这个对象的数据已被修改，需要保存/刷新
                for (int i = 0; i < texts.Length; i++)  //遍历所有被修改的TMP_Text，逐个标记为脏
                    EditorUtility.SetDirty(texts[i]);
            }
        }
    }
}
