/// <summary>
/// 全局入口：业务侧从这里取已初始化的系统，不要自己重复加载。
/// </summary>
public static class GameContext
{
    public static bool IsReady { get; private set; }

    public static ConsoleSystem Console { get; private set; }
    public static StaticDataSystem StaticData { get; private set; }
    public static UiSystem UI { get; private set; }

    public static void Bind(ConsoleSystem console, StaticDataSystem staticData, UiSystem ui)
    {
        Console = console;
        StaticData = staticData;
        UI = ui;
        IsReady = true;
    }

    public static void Clear()
    {
        Console = null;
        StaticData = null;
        UI = null;
        IsReady = false;
    }
}
