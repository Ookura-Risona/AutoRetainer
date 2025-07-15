namespace AutoRetainer.UI.NeoUI.MultiModeEntries;
public class MultiModeFPSLimiter : NeoUIEntry
{
    public override string Path => "多角色模式/FPS限制器";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("FPS限制器")
        .TextWrapped("FPS限制器仅在多角色模式启用时激活")
        .Widget("空闲时的目标帧率", (x) =>
        {
            ImGui.SetNextItemWidth(100f);
            UIUtils.SliderIntFrameTimeAsFPS(x, ref C.TargetMSPTIdle, C.ExtraFPSLockRange ? 1 : 10);
        })
        .Widget("空闲时的目标帧率", (x) =>
        {
            ImGui.SetNextItemWidth(100f);
            UIUtils.SliderIntFrameTimeAsFPS("操作时的目标帧率", ref C.TargetMSPTRunning, C.ExtraFPSLockRange ? 1 : 20);
        })
        .Checkbox("当游戏处于活动状态时释放FPS限制", () => ref C.NoFPSLockWhenActive)
        .Checkbox($"允许额外的低FPS限制值", () => ref C.ExtraFPSLockRange, "如果启用此选项并在多角色模式下遇到任何错误，将不提供支持")
        .Checkbox($"仅在设置关闭计时器时激活限制器", () => ref C.FpsLockOnlyShutdownTimer);
}
