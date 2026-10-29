using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏总启动器：按依赖顺序初始化各系统，并写入 GameContext。
/// 挂到启动场景的空物体上即可。
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    readonly List<IGameSystem> _systems = new List<IGameSystem>();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // 顺序 = 依赖顺序：静态表最先
        var staticData = new StaticDataSystem();
        _systems.Add(staticData);
        // 以后在此追加：AudioSystem、ConsoleSystem 等

        foreach (var sys in _systems)
        {
            Debug.Log($"[启动] 正在初始化：{sys.Name}");
            sys.Init();
        }

        GameContext.Bind(staticData);
        Debug.Log("[启动] 全部系统初始化完成");
    }

    void OnDestroy()
    {
        for (int i = _systems.Count - 1; i >= 0; i--)
            _systems[i].Shutdown();

        GameContext.Clear();
    }
}
