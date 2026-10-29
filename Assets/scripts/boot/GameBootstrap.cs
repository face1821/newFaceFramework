using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏总启动器：按依赖顺序初始化各系统，并写入 GameContext。
/// 挂到启动场景的空物体上即可。
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    readonly List<IGameSystem> _systems = new List<IGameSystem>();
    ConsoleSystem _console;

    void Awake() //初始化都在此Awake函数里，同场景的其他脚本尽量使用Start函数，否则执行顺序可能出现问题
    {
        DontDestroyOnLoad(gameObject);

        // 顺序 = 依赖顺序：控制台最先，便于收集后续系统日志
        _console = new ConsoleSystem(transform);
        _systems.Add(_console);

        var staticData = new StaticDataSystem();
        _systems.Add(staticData);

        foreach (var sys in _systems)
        {
            Debug.Log($"[启动] 正在初始化：{sys.Name}");
            sys.Init();
        }

        GameContext.Bind(_console, staticData);
        Debug.Log("[启动] 全部系统初始化完成");
    }

    void Update()
    {
        if (_console != null)
            _console.Tick();
    }

    void OnDestroy()
    {
        for (int i = _systems.Count - 1; i >= 0; i--)
            _systems[i].Shutdown();

        GameContext.Clear();
    }
}
