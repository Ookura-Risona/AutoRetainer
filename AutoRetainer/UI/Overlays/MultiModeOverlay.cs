using AutoRetainer.Modules.Voyage;
using Dalamud.Game.ClientState.Conditions;
using System.IO;

namespace AutoRetainer.UI.Overlays;

internal class MultiModeOverlay : Window
{
    public MultiModeOverlay() : base("AutoRetainer Alert", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoBackground, true)
    {
        P.WindowSystem.AddWindow(this);
        IsOpen = true;
        ShowCloseButton = false;
        RespectCloseHotkey = false;
    }

    private bool DisplayNotify => C.NotifyEnableOverlay && NotificationHandler.CurrentState && !NotificationHandler.IsHidden && (!C.NotifyCombatDutyNoDisplay || !(Svc.Condition[ConditionFlag.BoundByDuty56] || Svc.Condition[ConditionFlag.InCombat]));

    public override bool DrawConditions()
    {
        return !C.HideOverlayIcons && (P.TaskManager.IsBusy || P.IsNextToBell || MultiMode.Enabled || SchedulerMain.PluginEnabled || DisplayNotify || VoyageScheduler.Enabled || Shutdown.Active || SchedulerMain.CharacterPostProcessLocked);
    }

    private Vector2 StatusPanelSize => new(C.StatusBarIconWidth);

    public override void Draw()
    {
        var displayed = false;
        bool ShouldDisplay() => !displayed || !C.StatusBarMSI;
        CImGui.igBringWindowToDisplayBack(CImGui.igGetCurrentWindow());
        if(BailoutManager.IsLogOnTitleEnabled && ShouldDisplay())
        {
            displayed = true;
            if(ThreadLoadImageHandler.TryGetTextureWrap(Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, "res", "bailoutTitleRestart.png"), out var t))
            {
                ImGui.Image(t.Handle, StatusPanelSize);
                if(ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Left))
                    {
                        Svc.Commands.ProcessCommand("/ays");
                    }
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Right))
                    {
                        BailoutManager.IsLogOnTitleEnabled = false;
                    }
                    ImGui.SetTooltip($"AutoRetainer 正在等待登录界面选择有效角色。\n左键 - 打开 AutoRetainer\n右键 - 取消操作");
                }
            }
            else
            {
                ImGuiEx.Text($"loading bailoutTitleRestart.png");
            }
            ImGui.SameLine();
        }
        if(Shutdown.Active && ShouldDisplay())
        {
            displayed = true;
            if(ThreadLoadImageHandler.TryGetTextureWrap(Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, "res", "timer.png"), out var t))
            {
                ImGui.Image(t.Handle, StatusPanelSize);
                if(ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Left))
                    {
                        Svc.Commands.ProcessCommand("/ays");
                    }
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Right))
                    {
                        Shutdown.ForceShutdownAt = 0;
                        Shutdown.ShutdownAt = 0;
                    }
                    ImGui.SetTooltip($"已设置关机计时器。\n将在 {TimeSpan.FromMilliseconds(Shutdown.ShutdownAt - Environment.TickCount64)} 后关机\n将在 {TimeSpan.FromMilliseconds(Shutdown.ForceShutdownAt - Environment.TickCount64)} 后强制关机\n左键 - 打开 AutoRetainer\n右键 - 清除计时器");
                }
            }
            else
            {
                ImGuiEx.Text($"loading timer.png");
            }
            ImGui.SameLine();
        }
        if(SchedulerMain.CharacterPostProcessLocked && ShouldDisplay())
        {
            displayed = true;
            if(ThreadLoadImageHandler.TryGetTextureWrap(Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, "res", "processing.png"), out var t))
            {
                ImGui.Image(t.Handle, StatusPanelSize);
                if(ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Left))
                    {
                        Svc.Commands.ProcessCommand("/ays");
                    }
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Right))
                    {
                        SchedulerMain.CharacterPostProcessLocked = false;
                    }
                    ImGui.SetTooltip("AutoRetainer 正在执行后处理操作\n左键 - 打开 AutoRetainer\n右键 - 取消操作");
                }
            }
            else
            {
                ImGuiEx.Text($"loading multi.png");
            }
            ImGui.SameLine();
        }
        if(P.TaskManager.IsBusy && ShouldDisplay())
        {
            displayed = true;
            if(ThreadLoadImageHandler.TryGetTextureWrap(Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, "res", "processing.png"), out var t))
            {
                ImGui.Image(t.Handle, StatusPanelSize);
                if(ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Left))
                    {
                        Svc.Commands.ProcessCommand("/ays");
                    }
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Right))
                    {
                        P.TaskManager.Abort();
                    }
                    ImGui.SetTooltip("AutoRetainer 正在处理任务\n左键 - 打开 AutoRetainer\n右键 - 取消任务");
                }
            }
            else
            {
                ImGuiEx.Text($"loading multi.png");
            }
            ImGui.SameLine();
        }
        if(P.IsNextToBell && ShouldDisplay())
        {
            displayed = true;
            if(ThreadLoadImageHandler.TryGetTextureWrap(Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, "res", "bellalert.png"), out var t))
            {
                ImGui.Image(t.Handle, StatusPanelSize);
                if(ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Left))
                    {
                        Svc.Commands.ProcessCommand("/ays");
                    }
                    ImGui.SetTooltip("雇员感知功能已激活\n左键 - 打开 AutoRetainer");
                }
                var f = (float)(Environment.TickCount64 - P.LastMovementAt) / (float)C.RetainerSenseThreshold;
                ImGui.ProgressBar(f, new(128, 10), "");
            }
            else
            {
                ImGuiEx.Text($"loading bellalert.png");
            }
            ImGui.SameLine();
        }
        if(MultiMode.Enabled && ShouldDisplay())
        {
            displayed = true;
            if(ThreadLoadImageHandler.TryGetTextureWrap(Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, "res", "multi.png"), out var t))
            {
                ImGui.Image(t.Handle, StatusPanelSize);
                if(ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Left))
                    {
                        Svc.Commands.ProcessCommand("/ays");
                    }
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Right))
                    {
                        MultiMode.Enabled = false;
                    }
                    ImGui.SetTooltip("多角色模式已启用\n左键 - 打开 AutoRetainer\n右键 - 禁用多角色模式");
                }
            }
            else
            {
                ImGuiEx.Text($"loading multi.png");
            }
            ImGui.SameLine();
        }
        if(C.NightMode && MultiMode.Enabled && ShouldDisplay())
        {
            displayed = true;
            if(ThreadLoadImageHandler.TryGetTextureWrap(Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, "res", "Night.png"), out var t))
            {
                ImGui.Image(t.Handle, StatusPanelSize);
                if(ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Left))
                    {
                        Svc.Commands.ProcessCommand("/ays");
                    }
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Right))
                    {
                        C.NightMode = false;
                        MultiMode.BailoutNightMode();
                    }
                    ImGui.SetTooltip($"夜间模式已启用\n左键 - 打开 AutoRetainer\n右键 - 禁用夜间模式");
                }
            }
            else
            {
                ImGuiEx.Text($"loading Night.png");
            }
            ImGui.SameLine();
        }
        if(VoyageScheduler.Enabled && ShouldDisplay())
        {
            displayed = true;
            if(ThreadLoadImageHandler.TryGetTextureWrap(Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, "res", "submarine.png"), out var t))
            {
                ImGui.Image(t.Handle, StatusPanelSize);
                if(ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Left))
                    {
                        Svc.Commands.ProcessCommand("/ays");
                    }
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Right))
                    {
                        VoyageScheduler.Enabled = false;
                    }
                    ImGui.SetTooltip("潜艇模块已启用\n左键 - 打开 AutoRetainer\n右键 - 禁用潜艇模块");
                }
            }
            else
            {
                ImGuiEx.Text($"loading submarine.png");
            }
            ImGui.SameLine();
        }
        if(SchedulerMain.PluginEnabled && ShouldDisplay())
        {
            displayed = true;
            if(ThreadLoadImageHandler.TryGetTextureWrap(Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, "res", Utils.GetReachableRetainerBell(false) == null ? "bellcrossed.png" : "bell.png"), out var t))
            {
                ImGui.Image(t.Handle, StatusPanelSize);
                if(ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Left))
                    {
                        Svc.Commands.ProcessCommand("/ays");
                    }
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Right))
                    {
                        SchedulerMain.DisablePlugin();
                    }
                    ImGui.SetTooltip("AutoRetainer 已启用\n左键 - 打开 AutoRetainer\n右键 - 禁用 AutoRetainer");
                }
            }
            else
            {
                ImGuiEx.Text($"loading bell.png");
            }
            ImGui.SameLine();
        }
        if(DisplayNotify && ShouldDisplay())
        {
            displayed = true;
            if(ThreadLoadImageHandler.TryGetTextureWrap(Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, "res", "notify.png"), out var t))
            {
                ImGui.Image(t.Handle, StatusPanelSize);
                if(ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Left))
                    {
                        NotificationHandler.IsHidden = true;
                        Svc.Commands.ProcessCommand("/ays");
                    }
                    if(ImGui.IsItemClicked(ImGuiMouseButton.Right))
                    {
                        NotificationHandler.IsHidden = true;
                    }
                    ImGui.SetTooltip("有雇员已完成探险\n左键 - 打开 AutoRetainer\n右键 - 忽略通知");
                }
            }
            else
            {
                ImGuiEx.Text($"loading notify.png");
            }
            ImGui.SameLine();
        }
        ImGui.Dummy(Vector2.One);
        if(Data != null && !C.OldStatusIcons)
        {
            ImGuiEx.LineCentered("状态", delegate
            {
                if(C.MultiModeWorkshopConfiguration.MultiWaitForAll)
                {
                    if(ThreadLoadImageHandler.TryGetTextureWrap(Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, "res", "wait.png"), out var t))
                    {
                        ImGui.Image(t.Handle, StatusPanelSize / 2);
                        ImGuiEx.Tooltip("已全局启用等待所有远航探索功能");
                    }
                    else
                    {
                        ImGuiEx.Text($"loading wait.png");
                    }
                    ImGui.SameLine();
                }
                else if(Data.MultiWaitForAllDeployables)
                {
                    if(ThreadLoadImageHandler.TryGetTextureWrap(Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, "res", "wait.png"), out var t))
                    {
                        ImGui.Image(t.Handle, StatusPanelSize / 2);
                        ImGuiEx.Tooltip("已为此角色启用等待所有远航探索功能");
                    }
                    else
                    {
                        ImGuiEx.Text($"loading wait.png");
                    }
                    ImGui.SameLine();
                }
            });
        }

        Position = new(ImGuiHelpers.MainViewport.Size.X / 2 - ImGui.GetWindowSize().X / 2, 20);
    }
}
