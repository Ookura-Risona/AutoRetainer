namespace AutoRetainer.UI.NeoUI;
public class MainSettings : NeoUIEntry
{
    public override string Path => "常规";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("延迟设置")
        .Widget(100f, "时间不同步补偿", (x) => ImGuiEx.SliderInt(x, ref C.UnsyncCompensation.ValidateRange(-60, 0), -10, 0),
            "额外减少的秒数，用于缓解游戏与PC时间不同步可能导致的问题")
        .Widget(100f, "额外交互延迟（帧数）", (x) => ImGuiEx.SliderInt(x, ref C.ExtraFrameDelay.ValidateRange(-10, 100), 0, 50),
            "值越低插件操作越快。遇到低帧数或高延迟时可增加此值，如需更快操作可减少此值")
        .Widget("额外日志记录", (x) => ImGui.Checkbox(x, ref C.ExtraDebug), "启用调试用的详细日志记录，会产生大量日志并影响性能。插件重载或游戏重启后将自动禁用")

        .Section("操作模式")
        .Widget("分配 + 重新分配", (x) =>
        {
            if (ImGui.RadioButton(x, C.EnableAssigningQuickExploration && !C._dontReassign))
            {
                C.EnableAssigningQuickExploration = true;
                C.DontReassign = false;
            }
        }, "自动为启用的雇员分配自由探险（如果当前没有进行中的探险），并重新分配当前探险")
        .Widget("仅领取", (x) =>
        {
            if (ImGui.RadioButton(x, !C.EnableAssigningQuickExploration && C._dontReassign))
            {
                C.EnableAssigningQuickExploration = false;
                C.DontReassign = true;
            }
        }, "仅从雇员处领取探险奖励，不会重新分配任务\n按住CTRL键与雇员铃交互可临时启用此模式")
        .Widget("仅重新分配", (x) =>
        {
            if(ImGui.RadioButton("仅重新分配", !C.EnableAssigningQuickExploration && !C._dontReassign))
            {
                C.EnableAssigningQuickExploration = false;
                C.DontReassign = false;
            }
        }, "仅重新分配雇员当前进行的探险任务.")
        .Widget("雇员感知", (x) => ImGui.Checkbox(x, ref C.RetainerSense), "当玩家在雇员水晶交互范围内时，AutoRetainer将自动启用。必须保持静止状态否则激活将被取消")
        .Widget(200f, "激活时间（毫秒）", (x) => ImGuiEx.SliderIntAsFloat(x, ref C.RetainerSenseThreshold, 1000, 100000));


}
