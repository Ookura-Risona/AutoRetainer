namespace AutoRetainer.UI.NeoUI.MultiModeEntries;
public class MultiModeDeployables : NeoUIEntry
{
    public override string Path => "多角色模式/远航探索";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("多角色模式 - 远航探索")
        .Checkbox("等待航行完成", () => ref C.MultiModeWorkshopConfiguration.MultiWaitForAll, """启用后，AutoRetainer 将在登录角色前等待所有远航探索返回。如果您因其他原因已经登录，除非同时启用了全局设置"即使已登录也等待"，否则仍会重新派遣已完成的潜艇/飞空艇。""")
        .Indent()
        .Checkbox("即使已登录也等待", () => ref C.MultiModeWorkshopConfiguration.WaitForAllLoggedIn, """改变"等待航行完成"（包括全局和角色级设置）的行为，使AutoRetainer在已登录时不再单独重新派遣潜艇/飞空艇，而是等待所有单位返回后再进行操作。""")
        .InputInt(120f, "最大等待时间（分钟）", () => ref C.MultiModeWorkshopConfiguration.MaxMinutesOfWaiting.ValidateRange(0, 9999), 10, 60, """如果等待其他远航探索返回的时间会超过此分钟数，AutoRetainer将忽略"等待航行完成"和"即使已登录也等待"设置。""")
        .Unindent()
        .DragInt(60f, "提前登录阈值（秒）", () => ref C.MultiModeWorkshopConfiguration.AdvanceTimer.ValidateRange(0, 300), 0.1f, 0, 300, "AutoRetainer应在该角色上的潜艇/飞空艇准备好重新派遣前提前登录的秒数。")
        .DragInt(120f, "雇员任务处理截止时间（分钟）", () => ref C.DisableRetainerVesselReturn.ValidateRange(0, 60), "如果设置大于0的值，AutoRetainer将在此分钟数前停止处理任何雇员任务（考虑所有先前设置），以防任何角色重新部署潜艇/飞空艇。")
        .Checkbox("进入工坊时定期检查部队箱金币", () => ref C.FCChestGilCheck, "进入工坊时定期检查部队箱，以保持金币计数器更新。")
        .Indent()
        .SliderInt(150f, "检查频率（小时）", () => ref C.FCChestGilCheckCd, 0, 24 * 5)
        .Widget("重置冷却", (x) =>
        {
            if(ImGuiEx.Button(x, C.FCChestGilCheckTimes.Count > 0)) C.FCChestGilCheckTimes.Clear();
        })
        .Unindent();
}