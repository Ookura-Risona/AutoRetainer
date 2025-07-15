namespace AutoRetainer.UI.NeoUI.MultiModeEntries;
public class MultiModeDeployables : NeoUIEntry
{
    public override string Path => "多角色模式/远航探索";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("多角色模式 - 远航探索")
        .Checkbox("等待航行完成", () => ref C.MultiModeWorkshopConfiguration.MultiWaitForAll, "AutoRetainer将在多角色模式操作中等待所有远航探索完成后再切换到下一个角色。")
        .InputInt(120f, "最大等待时间（分钟）", () => ref C.MultiModeWorkshopConfiguration.MaxMinutesOfWaiting.ValidateRange(0, 9999), 10, 60, "如果等待其他远航探索完成的时间超过此分钟数，AutoRetainer将忽略此设置。")
        .DragInt(60f, "提前重登阈值", () => ref C.MultiModeWorkshopConfiguration.AdvanceTimer.ValidateRange(0, 300), 0.1f, 0, 300)
        .Checkbox("即使已登录也等待", () => ref C.MultiModeWorkshopConfiguration.WaitForAllLoggedIn)
        .DragInt(120f, "雇员探险处理截止时间", () => ref C.DisableRetainerVesselReturn.ValidateRange(0, 60), "部署航行剩余分钟数，用于阻止雇员任务处理。")
        .Checkbox("进入工坊时定期检查部队箱金币", () => ref C.FCChestGilCheck)
        .Indent()
        .SliderInt(150f, "检查频率（小时）", () => ref C.FCChestGilCheckCd, 0, 24 * 5)
        .Widget("重置冷却", (x) =>
        {
            if(ImGuiEx.Button(x, C.FCChestGilCheckTimes.Count > 0)) C.FCChestGilCheckTimes.Clear();
        })
        .Unindent();
}