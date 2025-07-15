namespace AutoRetainer.UI.NeoUI;
public class Keybinds : NeoUIEntry
{
    public override string Path => "快捷键";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("访问雇员铃/工坊面板快捷键")
        .Widget("在使用雇员铃/工坊面板时，临时阻止AutoRetainer自动启用", (x) =>
        {
            UIUtils.DrawKeybind(x, ref C.Suppress);
        })
        .Widget("临时设置为收集操作模式，阻止为当前周期分配探险任务/临时将可部署模式设置为仅最终化", (x) =>
        {
            UIUtils.DrawKeybind(x, ref C.TempCollectB);
        })

        .Section("快速雇员操作")
        .Widget("出售物品", (x) => UIUtils.QRA(x, ref C.SellKey))
        .Widget("委托物品", (x) => UIUtils.QRA(x, ref C.EntrustKey))
        .Widget("取回物品", (x) => UIUtils.QRA(x, ref C.RetrieveKey))
        .Widget("上架出售", (x) => UIUtils.QRA(x, ref C.SellMarketKey));
}
