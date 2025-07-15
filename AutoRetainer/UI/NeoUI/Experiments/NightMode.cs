namespace AutoRetainer.UI.NeoUI.Experiments;

internal class NightMode : ExperimentUIEntry
{
    public override string Name => "夜间模式";
    public override void Draw()
    {
        ImGuiEx.TextWrapped($"夜间模式:\n" +
                $"- 将强制启用登录界面等待选项\n" +
                $"- 将强制应用内置的FPS限制器\n" +
                $"- 当游戏窗口失去焦点且处于等待状态时，游戏帧率将被限制在0.2 FPS\n" +
                $"- 这看起来可能像是游戏卡住了，但重新激活游戏窗口后，请给予最多5秒的时间让其恢复\n" +
                $"- 默认情况下，夜间模式仅启用远航探索\n" +
                $"- 禁用夜间模式后，应急管理器将激活以将您重新登录回游戏");
        if(ImGui.Checkbox("激活夜间模式", ref C.NightMode)) MultiMode.BailoutNightMode();
        ImGui.Checkbox("显示夜间模式复选框", ref C.ShowNightMode);
        ImGui.Checkbox("在夜间模式下执行雇员任务", ref C.NightModeRetainers);
        ImGui.Checkbox("在夜间模式下执行远航探索", ref C.NightModeDeployables);
        ImGui.Checkbox("长期保持夜间模式状态", ref C.NightModePersistent);
        ImGui.Checkbox("使关机命令激活夜间模式而非关闭游戏", ref C.ShutdownMakesNightMode);
    }
}
