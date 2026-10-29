using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏内控制台面板：由 ConsoleSystem 在打开时实例化，关闭时销毁。
/// 预制体上需绑定 contentText；closeButton / clearButton / scrollRect 可选。
/// </summary>
public class ConsoleUI : MonoBehaviour
{
    [SerializeField] TMP_Text contentText;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] Button closeButton;
    [SerializeField] Button clearButton;

    readonly StringBuilder _builder = new StringBuilder(4096);

    void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(OnClickClose);
        if (clearButton != null)
            clearButton.onClick.AddListener(OnClickClear);
    }

    void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(OnClickClose);
        if (clearButton != null)
            clearButton.onClick.RemoveListener(OnClickClear);

        if (GameContext.Console != null)
            GameContext.Console.NotifyUIClosed(this);
    }

    /// <summary>
    /// 打开时一次性填入历史日志。
    /// </summary>
    public void Bind(IReadOnlyList<ConsoleLogEntry> entries)
    {
        _builder.Clear();
        if (entries != null)
        {
            for (int i = 0; i < entries.Count; i++)
                AppendLine(entries[i]);
        }

        ApplyText();
        ScrollToBottom();
    }

    /// <summary>
    /// 运行中追加一条日志。
    /// </summary>
    public void Append(ConsoleLogEntry entry)
    {
        if (entry == null)
            return;

        AppendLine(entry);
        ApplyText();
        ScrollToBottom();
    }

    public void ClearView()
    {
        _builder.Clear();
        ApplyText();
    }

    void AppendLine(ConsoleLogEntry entry)
    {
        string color = GetColor(entry.Type);
        string time = entry.Time.ToString("HH:mm:ss");
        _builder.Append("<color=#").Append(color).Append('>');
        _builder.Append('[').Append(time).Append("] ");
        _builder.Append(EscapeRichText(entry.Message));
        _builder.Append("</color>\n");

        if (entry.Type == LogType.Error || entry.Type == LogType.Exception || entry.Type == LogType.Assert)
        {
            if (!string.IsNullOrEmpty(entry.StackTrace))
            {
                _builder.Append("<color=#FF8888><size=80%>");
                _builder.Append(EscapeRichText(entry.StackTrace));
                _builder.Append("</size></color>\n");
            }
        }
    }

    void ApplyText()
    {
        if (contentText != null)
            contentText.text = _builder.ToString();
    }

    void ScrollToBottom()
    {
        if (scrollRect == null)
            return;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    static string GetColor(LogType type)
    {
        switch (type)
        {
            case LogType.Warning:
                return "FFCC00";
            case LogType.Error:
            case LogType.Exception:
            case LogType.Assert:
                return "FF5555";
            default:
                return "FFFFFF";
        }
    }

    static string EscapeRichText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        // 避免日志内容里的 < > 被 TMP 当成富文本标签
        return text.Replace("<", "＜").Replace(">", "＞");
    }

    void OnClickClose()
    {
        if (GameContext.Console != null)
            GameContext.Console.Close();
        else
            Destroy(gameObject);
    }

    void OnClickClear()
    {
        if (GameContext.Console != null)
            GameContext.Console.ClearLogs();
        else
            ClearView();
    }
}
