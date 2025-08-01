namespace AutoRetainer.UI.NeoUI.MultiModeEntries;
public class MultiModeRetainers : NeoUIEntry
{
    public override string Path => "多角色模式/雇员设置";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("多角色模式 - 雇员设置")
        .Checkbox("等待探险完成", () => ref C.MultiModeRetainerConfiguration.MultiWaitForAll, "AutoRetainer将在多角色模式操作中，等待所有雇员返回后才切换到下一个角色")
        .DragInt(60f, "提前重登阈值(秒)", () => ref C.MultiModeRetainerConfiguration.AdvanceTimer.ValidateRange(0, 300), 0.1f, 0, 300)
        .SliderInt(100f, "继续操作所需的最小背包空格", () => ref C.MultiMinInventorySlots.ValidateRange(2, 9999), 2, 30)
        .Checkbox("同步雇员状态（一次性）", () => ref MultiMode.Synchronize, "AutoRetainer 将等待所有启用的雇员完成其委托。完成后此设置将自动禁用，并处理所有角色。")
        .Checkbox($"强制执行完整角色轮换", () => ref C.CharEqualize, "推荐给拥有超过 15 个角色的用户使用，强制多角色模式确保在返回循环起点前，按顺序处理所有角色的委托。")
        .Indent()
        .Checkbox("按探险完成时间排序角色", () => ref C.LongestVentureFirst, "更早完成探险的角色将优先处理")
        .Checkbox("按雇员等级和上限排序角色", () => ref C.CappedLevelsLast, "首先处理有雇员可以升级的角色；然后处理雇员已达最大等级的角色；最后处理雇员未达最大等级但已到等级上限的角色")
        .Unindent();
}
