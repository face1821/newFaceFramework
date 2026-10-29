/// <summary>
/// 全局系统统一接口：由 GameBootstrap 按顺序启动/关闭。
/// </summary>
public interface IGameSystem
{
    string Name { get; }
    void Init();
    void Shutdown();
}
