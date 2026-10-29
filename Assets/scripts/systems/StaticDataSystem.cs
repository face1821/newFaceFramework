using System.IO;
using UnityEngine;
using Luban.SimpleJSON;
using cfg;

/// <summary>
/// 静态表数据系统：负责加载 Luban 生成的 Tables。
/// </summary>
public class StaticDataSystem : IGameSystem
{
    public string Name => "StaticData";

    public Tables Tables { get; private set; }

    public void Init()
    {
        string jsonDir = Path.Combine(Application.dataPath, "resource/jsondata");

        Tables = new Tables(file =>
        {
            string path = Path.Combine(jsonDir, file + ".json");
            string text = File.ReadAllText(path);
            return JSON.Parse(text);
        });

        Debug.Log("[静态数据] 配置表加载完成");
    }

    public void Shutdown()
    {
        Tables = null;
    }
}
