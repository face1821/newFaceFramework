using TMPro;
using UnityEngine;

/// <summary>
/// 编辑器工具：将指定 TMP 字体应用到本物体及所有子孙物体上的 TMP_Text。
/// </summary>
public class ApplyTmpFont : MonoBehaviour
{
    public TMP_FontAsset fontAsset;

    /// <summary>
    /// 递归替换本物体及所有子物体（含未激活）上的 TMP 字体。
    /// </summary>
    [ContextMenu("Apply TMP Font To Children")]
    public void ApplyFont()
    {
        if (fontAsset == null)
        {
            Debug.LogWarning($"[{name}] 未指定 Font Asset", this);
            return;
        }

        var texts = GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < texts.Length; i++)
            texts[i].font = fontAsset;

        Debug.Log($"[{name}] 已替换 {texts.Length} 个 TMP 文本的字体", this);
    }
}
