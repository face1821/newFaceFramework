using UnityEngine;

/// <summary>
/// 面板基类：由 UiSystem 调用 Open/Close，子类重写 OnOpen/OnClose。
/// </summary>
public abstract class UiPanel : MonoBehaviour
{
    [SerializeField] UiLayerType layer = UiLayerType.Panel;
    [SerializeField] string panelId;

    public UiLayerType Layer => layer;

    /// <summary>为空时用类型名。</summary>
    public string PanelId => string.IsNullOrEmpty(panelId) ? GetType().Name : panelId;

    public bool IsOpen { get; private set; }

    public void Open(object args = null)
    {
        IsOpen = true;
        OnOpen(args);
    }

    public void Close()
    {
        if (!IsOpen)
            return;

        OnClose();
        IsOpen = false;
    }

    protected virtual void OnOpen(object args) { }

    protected virtual void OnClose() { }

    protected virtual void OnDestroy()
    {
        if (GameContext.UI != null)
            GameContext.UI.NotifyPanelClosed(this);
    }
}
