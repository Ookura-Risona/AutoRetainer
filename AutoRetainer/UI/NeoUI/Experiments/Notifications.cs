namespace AutoRetainer.UI.NeoUI.Experiments;
public class Notifications : ExperimentUIEntry
{
    public override string Name => "通知";
    public override void Draw()
    {
        ImGui.Checkbox($"当有雇员完成探险时显示覆盖通知", ref C.NotifyEnableOverlay);
        ImGui.Checkbox($"在副本或战斗中不显示覆盖通知", ref C.NotifyCombatDutyNoDisplay);
        ImGui.Checkbox($"包含其他角色", ref C.NotifyIncludeAllChara);
        ImGui.Checkbox($"忽略未启用多角色模式的其他角色", ref C.NotifyIgnoreNoMultiMode);
        ImGui.Checkbox($"在游戏聊天栏显示通知", ref C.NotifyDisplayInChatX);
        ImGuiEx.Text($"当游戏处于非活动状态时: (需要安装并启用 NotificationMaster 插件)");
        ImGui.Checkbox($"发送桌面通知", ref C.NotifyDeskopToast);
        ImGui.Checkbox($"闪烁任务栏图标", ref C.NotifyFlashTaskbar);
        ImGui.Checkbox($"当插件运行时不发送通知", ref C.NotifyNoToastWhenRunning);
    }
}
