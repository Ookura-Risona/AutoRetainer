namespace AutoRetainer.UI.NeoUI.MultiModeEntries;
public class MultiModeRetainers : NeoUIEntry
{
    public override string Path => "多角色模式/雇员设置";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("多角色模式 - 雇员设置")
        .Checkbox("等待探险完成", () => ref C.MultiModeRetainerConfiguration.MultiWaitForAll, "AutoRetainer将在多角色模式操作中，等待所有雇员返回后才切换到下一个角色")
        .DragInt(60f, "提前重登阈值(秒)", () => ref C.MultiModeRetainerConfiguration.AdvanceTimer.ValidateRange(0, 300), 0.1f, 0, 300)
        .SliderInt(100f, "继续操作所需的最小背包空格", () => ref C.MultiMinInventorySlots.ValidateRange(2, 9999), 2, 30)
        .TextWrapped("以下排序设置也会影响潜艇的排序，尽管它们只读取雇员数据")
        .Indent()
        .Checkbox("按探险完成时间排序角色", () => ref C.LongestVentureFirst, "更早完成探险的角色将优先处理")
        .Checkbox("按雇员等级和上限排序角色", () => ref C.CappedLevelsLast, "首先处理有雇员可以升级的角色；然后处理雇员已达最大等级的角色；最后处理雇员未达最大等级但已到等级上限的角色")
        .Unindent();
}
