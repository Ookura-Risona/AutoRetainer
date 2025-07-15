using AutoRetainerAPI.Configuration;
using System.Collections.Frozen;

namespace AutoRetainer.UI.NeoUI.MultiModeEntries;
public class MultiModeContingency : NeoUIEntry
{
    private static readonly FrozenDictionary<WorkshopFailAction, string> WorkshopFailActionNames = new Dictionary<WorkshopFailAction, string>()
    {
        [WorkshopFailAction.StopPlugin] = "停止所有插件操作",
        [WorkshopFailAction.ExcludeVessel] = "排除该舰船",
        [WorkshopFailAction.ExcludeChar] = "排除该角色多角色模式循环",
    }.ToFrozenDictionary();

    public override string Path => "多角色模式/应急设置";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("应急设置")
        .TextWrapped("在此配置各种常见故障状态或潜在操作错误时的应急方案")
        .EnumComboFullWidth(null, "青磷水耗尽", () => ref C.FailureNoFuel, (x) => x != WorkshopFailAction.ExcludeVessel, WorkshopFailActionNames, "当青磷水不足无法派遣新航程时执行选择的应急操作")
        .EnumComboFullWidth(null, "无法维修舰艇", () => ref C.FailureNoRepair, null, WorkshopFailActionNames, "当魔导机械修理材料不足无法维修舰艇时执行选择的应急操作")
        .EnumComboFullWidth(null, "背包空间不足", () => ref C.FailureNoInventory, (x) => x != WorkshopFailAction.ExcludeVessel, WorkshopFailActionNames, "当角色背包空间不足无法接收航程奖励时执行选择的应急操作")
        .EnumComboFullWidth(null, "关键操作失败", () => ref C.FailureGeneric, (x) => x != WorkshopFailAction.ExcludeVessel, WorkshopFailActionNames, "当发生未知或杂项错误时执行选择的应急操作")
        .Widget("被GM监禁", (x) =>
        {
            ImGui.BeginDisabled();
            ImGuiEx.SetNextItemFullWidth();
            if(ImGui.BeginCombo("##jailsel", "终止游戏")) { ImGui.EndCombo(); }
            ImGui.EndDisabled();
        }, "当插件运行期间被GM监禁时选择执行的应急操作。祝你好运！");
}
