namespace AutoRetainer.UI.NeoUI;
public class MainSettings : NeoUIEntry
{
    public override string Path => "常规";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("延迟设置")
        .Widget(100f, "时间不同步补偿", (x) => ImGuiEx.SliderInt(x, ref C.UnsyncCompensation.ValidateRange(-60, 0), -10, 0), "额外减少的秒数，用于缓解游戏与PC时间不同步可能导致的问题")
        .Widget(100f, "额外交互延迟（帧数）", (x) => ImGuiEx.SliderInt(x, ref C.ExtraFrameDelay.ValidateRange(-10, 100), 0, 50), "值越低插件操作越快。遇到低帧数或高延迟时可增加此值，如需更快操作可减少此值")
        .Widget("额外日志记录", (x) => ImGui.Checkbox(x, ref C.ExtraDebug), "启用调试用的详细日志记录，会产生大量日志并影响性能。插件重载或游戏重启后将自动禁用")

            .Section("操作模式")
        .Widget("分配 + 重新分配", (x) =>
        {
            if(ImGui.RadioButton(x, C.EnableAssigningQuickExploration && !C._dontReassign))
            {
                C.EnableAssigningQuickExploration = true;
                C.DontReassign = false;
            }
        }, "自动为启用的雇员分配自由探险（如果当前没有进行中的探险），并重新分配当前探险")
        .Widget("仅领取", (x) =>
        {
            if(ImGui.RadioButton(x, !C.EnableAssigningQuickExploration && C._dontReassign))
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
        .Widget(200f, "激活时间（毫秒）", (x) => ImGuiEx.SliderIntAsFloat(x, ref C.RetainerSenseThreshold, 1000, 100000))

        .Section("用户界面")
        .Checkbox("匿名化雇员名称", () => ref C.NoNames, "雇员名称将在常规UI元素中被隐藏（调试菜单和插件日志中仍可见）。启用此选项后，不同插件界面中的角色和雇员编号可能不一致")
        .Checkbox("在雇员界面显示快捷菜单", () => ref C.UIBar)
        //.Checkbox("Opt out of custom Dalamud theme", () => ref C.NoTheme)
        .Checkbox("显示扩展雇员信息", () => ref C.ShowAdditionalInfo, "在主界面显示雇员装等/获得力/鉴别力及其当前探险名称")
        .Widget("按ESC键时不关闭AutoRetainer窗口", (x) =>
        {
            if(ImGui.Checkbox(x, ref C.IgnoreEsc)) Utils.ResetEscIgnoreByWindows();
        })
        .Checkbox("在状态栏仅显示最高优先级图标", () => ref C.StatusBarMSI)
        .SliderInt(120f, "状态栏图标大小", () => ref C.StatusBarIconWidth, 32, 128)
        .Checkbox("游戏启动时打开AutoRetainer窗口", () => ref C.DisplayOnStart)
        .Checkbox("插件激活时跳过物品出售/交易确认", () => ref C.SkipItemConfirmations)
        .Checkbox("启用标题界面按钮（需重启插件）", () => ref C.UseTitleScreenButton)
        .Checkbox("隐藏角色搜索", () => ref C.NoCharaSearch)
        .Checkbox("不为已完成角色显示背景闪烁", () => ref C.NoGradient)
        .Checkbox("不警告同一目录运行多个游戏实例", () => ref C.No2ndInstanceNotify, "这将使AutoRetainer在第二个游戏实例中自动跳过加载，除非在主要实例中禁用此选项");
}
