using UnityEngine;
using cfg;

public class LubanTest : MonoBehaviour
{
    void Start()
    {
        if (!GameContext.IsReady)
        {
            Debug.LogError("[测试] 游戏尚未初始化完成，请确认场景中已挂载 GameBootstrap");
            return;
        }

        FightBuffNewJsonInfo buff = GameContext.StaticData.Tables.TbFightBuffNewJsonInfo.Get(4301202);
        Debug.Log($"[测试] id={buff.Id}, name={buff.Name}, buffType={buff.BuffType}");
    }
}
