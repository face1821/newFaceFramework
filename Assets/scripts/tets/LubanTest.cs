using System.IO;
using UnityEngine;
using Luban.SimpleJSON;
using cfg;

public class LubanTest : MonoBehaviour
{
    void Start()
    {
        // JSON 在 Assets/resource/jsondata，编辑器下可用 dataPath 直接读
        string jsonDir = Path.Combine(Application.dataPath, "resource/jsondata");

        var tables = new Tables(file =>
        {
            string path = Path.Combine(jsonDir, file + ".json");
            string text = File.ReadAllText(path);
            return JSON.Parse(text);
        });

        // 按 id 取一行 buff 配置（表里第一条是 4301201）
        FightBuffNewJsonInfo buff = tables.TbFightBuffNewJsonInfo.Get(4301201);
        Debug.Log($"[LubanTest] id={buff.Id}, name={buff.Name}, buffType={buff.BuffType}");
    }
}
