using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>
/// UI 管家：维护 UIRoot 分层，负责面板的打开/关闭（同类型同时只开一个）。
/// </summary>
public class UiSystem : IGameSystem
{
    public string Name => "UI";

    static readonly int[] LayerOrders = { 100, 200, 300, 500 };

    readonly Transform _parent;
    readonly Dictionary<Type, string> _paths = new Dictionary<Type, string>();
    readonly Dictionary<Type, UiPanel> _opened = new Dictionary<Type, UiPanel>();
    readonly Dictionary<UiLayerType, Transform> _layers = new Dictionary<UiLayerType, Transform>();

    Transform _root;

    public UiSystem(Transform parent)
    {
        _parent = parent;
    }

    public void Init()
    {
        CreateRoot();
    }

    public void Shutdown()
    {
        CloseAll();
        _paths.Clear();
        _layers.Clear();

        if (_root != null)
        {
            Object.Destroy(_root.gameObject);
            _root = null;
        }
    }

    /// <summary>登记面板类型对应的 Resources 路径，例如 Prefabs/UI/TestPanel。</summary>
    public void Register<T>(string resourcesPath) where T : UiPanel
    {
        if (string.IsNullOrEmpty(resourcesPath))
        {
            Debug.LogError("[UI] Register 路径为空");
            return;
        }

        _paths[typeof(T)] = resourcesPath;
    }

    public T Open<T>(object args = null) where T : UiPanel
    {
        var type = typeof(T);
        if (_opened.TryGetValue(type, out var existing) && existing != null)
            return existing as T;

        if (!_paths.TryGetValue(type, out var path))
        {
            Debug.LogError($"[UI] 未 Register：{type.Name}");
            return null;
        }

        var prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"[UI] 加载失败：Resources/{path}");
            return null;
        }

        var panel = prefab.GetComponent<T>();
        if (panel == null)
        {
            Debug.LogError($"[UI] 预制体缺少组件：{type.Name}");
            return null;
        }

        if (!_layers.TryGetValue(panel.Layer, out var layerParent) || layerParent == null)
        {
            Debug.LogError($"[UI] 找不到层级：{panel.Layer}");
            return null;
        }

        var go = Object.Instantiate(prefab, layerParent);
        go.name = panel.PanelId;

        var instance = go.GetComponent<T>();
        if (instance == null)
        {
            Debug.LogError($"[UI] 实例缺少组件：{type.Name}");
            Object.Destroy(go);
            return null;
        }

        _opened[type] = instance;
        instance.Open(args);
        return instance;
    }

    public void Close<T>() where T : UiPanel
    {
        Close(typeof(T));
    }

    public void Close(UiPanel panel)
    {
        if (panel == null)
            return;
        Close(panel.GetType(), panel);
    }

    public bool IsOpen<T>() where T : UiPanel
    {
        return _opened.TryGetValue(typeof(T), out var panel) && panel != null;
    }

    public T Get<T>() where T : UiPanel
    {
        if (_opened.TryGetValue(typeof(T), out var panel) && panel != null)
            return panel as T;
        return null;
    }

    public void CloseAll()
    {
        var types = new List<Type>(_opened.Keys);
        for (int i = 0; i < types.Count; i++)
            Close(types[i]);
    }

    public void NotifyPanelClosed(UiPanel panel)
    {
        if (panel == null)
            return;

        var type = panel.GetType();
        if (_opened.TryGetValue(type, out var current) && current == panel)
            _opened.Remove(type);
    }

    public Transform GetLayer(UiLayerType layer)
    {
        _layers.TryGetValue(layer, out var t);
        return t;
    }

    void Close(Type type, UiPanel expected = null)
    {
        if (!_opened.TryGetValue(type, out var panel) || panel == null)
        {
            _opened.Remove(type);
            return;
        }

        if (expected != null && panel != expected)
            return;

        _opened.Remove(type);
        panel.Close();
        Object.Destroy(panel.gameObject);
    }

    void CreateRoot()
    {
        var rootGo = new GameObject("UIRoot");
        rootGo.transform.SetParent(_parent, false);
        _root = rootGo.transform;

        var values = (UiLayerType[])Enum.GetValues(typeof(UiLayerType));
        for (int i = 0; i < values.Length; i++)
        {
            var layer = values[i];
            int order = i < LayerOrders.Length ? LayerOrders[i] : 100 + i * 100;
            _layers[layer] = CreateLayer(layer.ToString(), order);
        }
    }

    Transform CreateLayer(string name, int sortingOrder)
    {
        var go = new GameObject(name);
        go.transform.SetParent(_root, false);

        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        go.AddComponent<GraphicRaycaster>();
        return go.transform;
    }
}
