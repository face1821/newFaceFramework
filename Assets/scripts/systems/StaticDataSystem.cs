using UnityEngine;
using Luban.SimpleJSON;
using cfg;

/// <summary>
/// 静态表数据系统：负责加载 Luban 生成的 Tables。
/// </summary>
public class StaticDataSystem : IGameSystem
{
    public string Name => "StaticData";

    const string JsonResourcesDir = "jsondata";

    public Tables Tables { get; private set; }

    public void Init()
    {
        Tables = new Tables(file =>
        {
            string path = JsonResourcesDir + "/" + file;
            TextAsset asset = Resources.Load<TextAsset>(path);
            if (asset == null)
            {
                Debug.LogError($"[静态数据] 加载失败：Resources/{path}");
                return JSON.Parse("{}");
            }

            return JSON.Parse(asset.text);
        });

        Debug.Log("[静态数据] 配置表加载完成");
    }

    public void Shutdown()
    {
        Tables = null;
    }
}
