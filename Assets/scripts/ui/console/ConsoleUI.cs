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

        bool stickToBottom = IsNearBottom();
        AppendLine(entry);
        ApplyText();
        if (stickToBottom)
            ScrollToBottom();
    }

    public void ClearView()
    {
        _builder.Clear();
        ApplyText();
    }

    void AppendLine(ConsoleLogEntry entry)
    {
        string time = entry.Time.ToString("HH:mm:ss");
        _builder.Append('[').Append(time).Append("] ");
        _builder.Append(entry.Message ?? string.Empty);
        _builder.Append('\n');

        if (entry.Type == LogType.Error || entry.Type == LogType.Exception || entry.Type == LogType.Assert)
        {
            if (!string.IsNullOrEmpty(entry.StackTrace))
            {
                _builder.Append(entry.StackTrace);
                _builder.Append('\n');
            }
        }
    }

    void ApplyText()
    {
        if (contentText != null)
            contentText.text = _builder.ToString();
    }

    bool IsNearBottom()
    {
        if (scrollRect == null)
            return true;
        return scrollRect.verticalNormalizedPosition <= 0.01f;
    }

    void ScrollToBottom()
    {
        if (scrollRect == null)
            return;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
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
