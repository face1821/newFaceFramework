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
        // for(int i = 0; i < 20; i++) {
        //     Debug.Log($"[测试] id={buff.Id}, name={buff.Name}, buffType={buff.BuffType}");
        // }
    }



    // float _timer;
    // int _count;
    // const float Interval = 1f / 60f;
    // void Update()
    // {
    //     _timer += Time.deltaTime;
    //     while (_timer >= Interval)
    //     {
    //         _timer -= Interval;
    //         _count++;
    //         Debug.Log($"测试输出{_count}");
    //     }
    // }
}

