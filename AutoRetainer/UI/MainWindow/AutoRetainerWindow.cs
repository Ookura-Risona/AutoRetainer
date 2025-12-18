using AutoRetainer.Modules.Voyage;
using AutoRetainer.UI.MainWindow.MultiModeTab;
using AutoRetainerAPI;
using AutoRetainerAPI.Configuration;
using Dalamud.Interface.Components;
using ECommons.Configuration;
using ECommons.Funding;
using NightmareUI;

namespace AutoRetainer.UI.MainWindow;

internal unsafe class AutoRetainerWindow : Window
{
    private TitleBarButton LockButton;

    public AutoRetainerWindow() : base($"")
    {
        PatreonBanner.IsOfficialPlugin = () => true;
        LockButton = new()
        {
            Click = OnLockButtonClick,
            Icon = C.PinWindow ? FontAwesomeIcon.Lock : FontAwesomeIcon.LockOpen,
            IconOffset = new(3, 2),
            ShowTooltip = () => ImGui.SetTooltip("锁定窗口位置和大小"),
        };
        SizeConstraints = new()
        {
            MinimumSize = new(250, 100),
            MaximumSize = new(9999, 9999)
        };
        P.WindowSystem.AddWindow(this);
        AllowPinning = false;
        TitleBarButtons.Add(new()
        {
            Click = (m) => { if(m == ImGuiMouseButton.Left) S.NeoWindow.IsOpen = true; },
            Icon = FontAwesomeIcon.Cog,
            IconOffset = new(2, 2),
            ShowTooltip = () => ImGui.SetTooltip("打开设置窗口"),
        });
        TitleBarButtons.Add(LockButton);
    }

    private Action<string> SomeAction;

    private void OnLockButtonClick(ImGuiMouseButton m)
    {
        SomeAction += (s) => { };
        SomeAction -= (s) => { };
        if(m == ImGuiMouseButton.Left)
        {
            C.PinWindow = !C.PinWindow;
            LockButton.Icon = C.PinWindow ? FontAwesomeIcon.Lock : FontAwesomeIcon.LockOpen;
        }
    }

    public override void PreDraw()
    {
        var prefix = SchedulerMain.PluginEnabled ? $" [{SchedulerMain.Reason}]" : "";
        var tokenRem = TimeSpan.FromMilliseconds(Utils.GetRemainingSessionMiliSeconds());
        WindowName = $"{P.Name} {P.GetType().Assembly.GetName().Version}{prefix} | {FormatToken(tokenRem)}###AutoRetainer";
        if(C.PinWindow)
        {
            ImGuiHelpers.SetNextWindowPosRelativeMainViewport(C.WindowPos);
            ImGui.SetNextWindowSize(C.WindowSize);
        }
    }

    private string FormatToken(TimeSpan time)
    {
        if(time.TotalMilliseconds > 0)
        {
            if(time.Days > 0)
            {
                return $"会话将在 {time.Days} 天" + (time.Hours > 0 ? $" {time.Hours} 时 后过期" : "");
            }
            else
            {
                if(time.Hours > 0)
                {
                    return $"会话将在 {time.Hours} 时后过期";
                }
                else
                {
                    return $"会话将在一个小时内过期";
                }
            }
        }
        else
        {
            return "会话已过期";
        }
    }
    public override void Draw()
    {
        //ImGuiEx.Text(GradientColor.Get(EColor.RedBright, EColor.YellowBright), "This version MUST NOT BE RUNNING UNATTENDED.");
        try
        {
            if(!C.AcceptedDisclamer)
            {
                new NuiBuilder()
                    .Section("免责声明")
                    .TextWrapped(ImGuiColors.DalamudYellow, "请注意，严格禁止出于RMT目的使用AutoRetainer 。")
                    .TextWrapped(ImGuiColors.DalamudRed, "为避免不必要的后果，使用AutoRetainer时请遵守以下规则 :")
                    .TextWrapped("1. 不要在游戏聊天中提及您使用AutoRetainer ；")
                    .TextWrapped("2. 不要长时间无人值守地运行AutoRetainer ；")
                    .TextWrapped("3. 确保您的实际游戏+AutoRetainer使用时间每天不超过16小时；确保在雇员/潜艇检查流程之间存在非活动间隔；")
                    .TextWrapped("4. 永远不要回应那些试图通过交易或聊天进行所谓'机器人检测'的玩家；应立即将这些玩家加入黑名单；")
                    .TextWrapped("5. 如果被GM询问，始终声称所有操作都是手动完成的，绝不承认使用插件。")
                    .TextWrapped("违反这些规则可能导致您的账号受到处罚。")
                    .TextWrapped(GradientColor.Get(ImGuiColors.DalamudYellow, ImGuiColors.DalamudRed), "您不得将 AutoRetainer 用于RMT行为或其他商业用途。如果您将其用于上述用途，我们将不提供任何支持。")
                    .Widget(() =>
                    {
                        if(ImGuiEx.IconButtonWithText(FontAwesomeIcon.Check, "接受并继续"))
                        {
                            C.AcceptedDisclamer = true;
                            EzConfig.Save();
                        }
                    })
                    .Draw();
                return;
            }
            var e = SchedulerMain.PluginEnabledInternal;
            var disabled = MultiMode.Active && !ImGui.GetIO().KeyCtrl;

            if(disabled)
            {
                ImGui.BeginDisabled();
            }
            if(ImGui.Checkbox($"启用 {P.Name}", ref e))
            {
                P.WasEnabled = false;
                if(e)
                {
                    SchedulerMain.EnablePlugin(PluginEnableReason.Auto);
                }
                else
                {
                    SchedulerMain.DisablePlugin();
                }
            }
            if(C.ShowDeployables && (VoyageUtils.Workshops.Contains(Svc.ClientState.TerritoryType) || VoyageScheduler.Enabled))
            {
                ImGui.SameLine();
                ImGui.Checkbox($"远航探索", ref VoyageScheduler.Enabled);
            }
            if(disabled)
            {
                ImGui.EndDisabled();
                ImGuiComponents.HelpMarker($"此选项由多角色模式控制。按住CTRL可覆盖。");
            }

            if(P.WasEnabled)
            {
                ImGui.SameLine();
                ImGuiEx.Text(GradientColor.Get(ImGuiColors.DalamudGrey, ImGuiColors.DalamudGrey3, 500), $"已暂停");
            }

            ImGui.SameLine();
            if(ImGui.Checkbox("多角色", ref MultiMode.Enabled))
            {
                MultiMode.OnMultiModeEnabled();
            }
            Utils.DrawLifestreamAvailabilityIndicator();
            if(C.ShowNightMode)
            {
                ImGui.SameLine();
                if(ImGui.Checkbox("夜间模式", ref C.NightMode))
                {
                    MultiMode.BailoutNightMode();
                }
            }
            if(C.DisplayMMType)
            {
                ImGui.SameLine();
                ImGuiEx.SetNextItemWidthScaled(100f);
                ImGuiEx.EnumCombo("##mode", ref C.MultiModeType);
            }
            if(C.CharEqualize && MultiMode.Enabled)
            {
                ImGui.SameLine();
                if(ImGui.Button("重置计数器"))
                {
                    MultiMode.CharaCnt.Clear();
                }
            }

            Svc.PluginInterface.GetIpcProvider<object>(ApiConsts.OnMainControlsDraw).SendMessage();

            if(IPC.Suppressed)
            {
                ImGuiEx.Text(ImGuiColors.DalamudRed, $"插件操作已被其他插件停止");
                ImGui.SameLine();
                if(ImGui.SmallButton("取消"))
                {
                    IPC.Suppressed = false;
                }
            }

            if(P.TaskManager.IsBusy)
            {
                ImGui.SameLine();
                if(ImGui.Button($"中止 {P.TaskManager.NumQueuedTasks} 个任务"))
                {
                    P.TaskManager.Abort();
                }
            }

            PatreonBanner.DrawRight();
            ImGuiEx.EzTabBar("标签栏", PatreonBanner.Text,
                            ("雇员管理", MultiModeUI.Draw, null, true),
                            ("远航探索", WorkshopUI.Draw, null, true),
                            ("故障排除", TroubleshootingUI.Draw, null, true),
                            ("统计信息", DrawStats, null, true),
                            ("关于", CustomAboutTab.Draw, null, true)
                            );
            if(!C.PinWindow)
            {
                C.WindowPos = ImGui.GetWindowPos();
                C.WindowSize = ImGui.GetWindowSize();
            }
        }
        catch(Exception e)
        {
            ImGuiEx.TextWrapped(e.ToStringFull());
        }
    }

    private void DrawStats()
    {
        NuiTools.ButtonTabs([[C.RecordStats ? new("雇员派遣", S.VentureStats.DrawVentures) : null, new("金币", S.GilDisplay.Draw), new("部队数据", S.FCData.Draw)]]);
    }

    public override void OnClose()
    {
        EzConfig.Save();
        S.VentureStats.Data.Clear();
        MultiModeUI.JustRelogged = false;
    }

    public override void OnOpen()
    {
        MultiModeUI.JustRelogged = true;
    }
}
