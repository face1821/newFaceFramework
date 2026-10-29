using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏内控制台系统：常驻缓存日志，双击 ` 键按需创建/销毁 ConsoleUI 预制体。
/// </summary>
public class ConsoleSystem : IGameSystem
{
    public string Name => "Console";

    const string ConsolePrefabPath = "Prefabs/Canvas";
    const int MaxEntries = 500;
    const float DoubleTapWindow = 0.35f;

    readonly List<ConsoleLogEntry> _entries = new List<ConsoleLogEntry>(128);
    readonly object _lock = new object();
    readonly List<ConsoleLogEntry> _pendingFromThread = new List<ConsoleLogEntry>(32);

    readonly Transform _uiParent;
    GameObject _uiPrefab;
    ConsoleUI _ui;

    float _lastBackQuoteTime = -999f;

    public bool IsOpen => _ui != null;

    public ConsoleSystem(Transform uiParent)
    {
        _uiParent = uiParent;
    }

    public void Init()
    {
        _uiPrefab = Resources.Load<GameObject>(ConsolePrefabPath);
        if (_uiPrefab == null)
            Debug.LogError($"[控制台] 加载失败：Resources/{ConsolePrefabPath}");

        Application.logMessageReceivedThreaded += OnUnityLogThreaded;
    }

    public void Shutdown()
    {
        Application.logMessageReceivedThreaded -= OnUnityLogThreaded;
        Close();
        lock (_lock)
        {
            _entries.Clear();
            _pendingFromThread.Clear();
        }
        _uiPrefab = null;
    }

    /// <summary>
    /// 由 GameBootstrap.Update 每帧调用：冲刷线程日志 + 检测双击开关键。
    /// </summary>
    public void Tick()
    {
        FlushPendingLogs();

        if (!Input.GetKeyDown(KeyCode.BackQuote))
            return;

        float now = Time.unscaledTime;
        if (now - _lastBackQuoteTime <= DoubleTapWindow)
        {
            Toggle();
            _lastBackQuoteTime = -999f;
        }
        else
        {
            _lastBackQuoteTime = now;
        }
    }

    /// <summary>
    /// 手动打印日志；内部走 Debug.Log，由 Unity 回调统一写入缓存（编辑器控制台也能看到）。
    /// </summary>
    public void Log(string message)
    {
        Debug.Log(message);
    }

    public void Warning(string message)
    {
        Debug.LogWarning(message);
    }

    public void Error(string message)
    {
        Debug.LogError(message);
    }

    public void Toggle()
    {
        if (IsOpen)
            Close();
        else
            Open();
    }

    public void Open()
    {
        if (_ui != null)
            return;

        if (_uiPrefab == null)
        {
            Debug.LogError($"[控制台] 预制体未加载：Resources/{ConsolePrefabPath}");
            return;
        }

        FlushPendingLogs();

        GameObject go = Object.Instantiate(_uiPrefab, _uiParent);
        go.name = "ConsoleUI";
        _ui = go.GetComponent<ConsoleUI>();
        if (_ui == null)
        {
            Debug.LogError("[控制台] 预制体上缺少 ConsoleUI 组件");
            Object.Destroy(go);
            return;
        }

        List<ConsoleLogEntry> snapshot;
        lock (_lock)
            snapshot = new List<ConsoleLogEntry>(_entries);

        _ui.Bind(snapshot);
    }

    public void Close()
    {
        if (_ui == null)
            return;

        Object.Destroy(_ui.gameObject);
        _ui = null;
    }

    public void NotifyUIClosed(ConsoleUI ui)
    {
        if (_ui == ui)
            _ui = null;
    }

    public void ClearLogs()
    {
        lock (_lock)
        {
            _entries.Clear();
            _pendingFromThread.Clear();
        }

        if (_ui != null)
            _ui.ClearView();
    }

    void OnUnityLogThreaded(string condition, string stackTrace, LogType type)
    {
        var entry = new ConsoleLogEntry(type, condition, stackTrace);
        lock (_lock)
            _pendingFromThread.Add(entry);
    }

    void FlushPendingLogs()
    {
        List<ConsoleLogEntry> batch;
        lock (_lock)
        {
            if (_pendingFromThread.Count == 0)
                return;
            batch = new List<ConsoleLogEntry>(_pendingFromThread);
            _pendingFromThread.Clear();
        }

        for (int i = 0; i < batch.Count; i++)
            AddEntry(batch[i]);
    }

    void AddEntry(ConsoleLogEntry entry)
    {
        lock (_lock)
        {
            _entries.Add(entry);
            while (_entries.Count > MaxEntries)
                _entries.RemoveAt(0);
        }

        if (_ui != null)
            _ui.Append(entry);
    }
}
