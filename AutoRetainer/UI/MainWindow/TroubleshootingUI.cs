using AutoRetainer.Modules.Voyage;
using Dalamud.Game;
using ECommons.GameHelpers;
using ECommons.Reflection;

namespace AutoRetainer.UI.MainWindow;
public static unsafe class TroubleshootingUI
{
    private static readonly Config EmptyConfig = new();

    public static bool IsPluginInstalled(string name)
    {
        return Svc.PluginInterface.InstalledPlugins.Any(x => x.IsLoaded && (x.InternalName.EqualsIgnoreCase(name) || x.Name.EqualsIgnoreCase(name)));
    }

    public static void Draw()
    {
        ImGuiEx.TextWrapped("本选项卡检查您的配置是否存在常见问题，您可以在联系技术支持前自行解决这些问题。");

        if(IsPluginInstalled("LightlessSync"))

        if(!Player.Available)
        {
            ImGuiEx.TextWrapped($"Can not troubleshoot when not logged in.");
            return;
        }

        if(Data == null)
        {
            ImGuiEx.TextWrapped($"No data available for current character. Access retainer bell, deployables panel or logout to create data.");
            return;
        }

        if(!Svc.ClientState.ClientLanguage.EqualsAny(ClientLanguage.Japanese, ClientLanguage.German, ClientLanguage.French, ClientLanguage.English))
        {
            Error($"检测到非国际服客户端。AutoRetainer未在其它最终幻想14客户端上进行测试。部分或全部功能可能无法正常工作。");
        }

        if(C.DontLogout)
        {
            Error("已启用DontLogout调试选项");
        }

        foreach(var x in C.OfflineData)
        {
            if(x.WorkshopEnabled)
            {
                var a = x.OfflineSubmarineData.Select(x => x.Name);
                if(a.Count() > a.Distinct().Count())
                {
                    Error($"角色 {Censor.Character(x.Name, x.World)} 的潜艇名称存在重复。潜艇名称必须唯一。");
                }
            }
        }

        if((C.GlobalTeleportOptions.Enabled || C.OfflineData.Any(x => x.TeleportOptionsOverride.Enabled == true)) && !Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == "Lifestream" && x.IsLoaded))
        {
            Error("\"已启用传送功能，但未安装或未启用Lifestream插件。AutoRetainer无法在此状态下运行。请禁用传送功能或安装Lifestream插件。");
        }

        foreach(var x in C.SubmarineUnlockPlans)
        {
            if(x.EnforcePlan)
            {
                Info($"潜艇解锁计划 {x.Name.NullWhenEmpty() ?? x.GUID} 设置为强制执行模式，如有需要解锁的内容，将覆盖所有潜艇设置。");
            }
        }

        foreach(var x in C.SubmarineUnlockPlans)
        {
            if(x.EnforceDSSSinglePoint)
            {
                Info($"潜艇解锁计划 {x.Name.NullWhenEmpty() ?? x.GUID} 设置为在深海站点单点部署，并将忽略手动设置的解锁行为。");
            }
        }

        try
        {
            if(DalamudReflector.IsOnStaging())
            {
                Error($"检测到非正式版Dalamud分支。这可能导致问题。请通过输入/xlbranch打开分支切换器，切换到\\\"release\\\"分支并重启游戏。");
            }
        }
        catch(Exception e)
        {
        }

        if(Player.Available)
        {
            if(Player.CurrentWorld != Player.HomeWorld)
            {
                Error("您正在访问其他服务器。必须返回原始服务器后，AutoRetainer才能继续处理此角色。");
            }
            if(C.Blacklist.Any(x => x.CID == Player.CID))
            {
                Error("当前角色已被完全排除在AutoRetainer处理之外。请前往设置→排除项进行更改。");
            }
            if(Data.ExcludeRetainer)
            {
                Error("当前角色已被排除在雇员列表外。请前往设置→排除项进行更改。");
            }
            if(Data.ExcludeWorkshop)
            {
                Error("当前角色已被排除在远航探索列表外。请前往设置→排除项进行更改。");
            }
        }

        {
            var list = C.OfflineData.Where(x => x.GetAreTeleportSettingsOverriden());
            if(list.Any())
            {
                Info("部分角色的传送选项已自定义。悬停查看列表。", list.Select(x => $"{x.Name}@{x.World}").Print("\n"));
            }
        }

        if(C.NoTeleportHetWhenNextToBell)
        {
            Warning("当角色靠近雇员铃时，传送或进入房屋/公寓的功能已被禁用。请注意房屋拆除计时器。");
        }



        if(C.AllowSimpleTeleport)
        {
            Warning("已启用简单传送选项。此选项不如在Lifestream中登记房屋可靠。如遇传送问题，请考虑禁用此选项并在Lifestream中登记您的房产。");
        }

        if(!C.EnableEntrustManager && C.AdditionalData.Any(x => x.Value.EntrustPlan != Guid.Empty))
        {
            Warning($"托管管理器已全局禁用，但部分雇员已分配托管计划。托管计划将仅在手动操作时处理。");
        }

        if(C.ExtraDebug)
        {
            Info("已启用额外日志记录选项。这将导致日志大量输出，请仅在收集调试信息时使用。");
        }

        if(C.UnsyncCompensation > -5)
        {
            Warning("时间不同步补偿值设置过高(>-5)，可能导致问题。");
        }

        if(UIUtils.GetFPSFromMSPT(C.TargetMSPTIdle) < 10)
        {
            Warning("空闲时目标帧率设置过低(<10)，可能导致问题。");
        }

        if(UIUtils.GetFPSFromMSPT(C.TargetMSPTRunning) < 20)
        {
            Warning("运行时的目标帧率设置过低(<20)，可能导致问题。");
        }

        if(Data?.GetIMSettings().AllowSellFromArmory == true)
        {
            Info("已启用允许从装备兵装库出售物品选项。请确保将您的零式装备和绝境武器加入保护列表。");
        }

        {
            var list = C.OfflineData.Where(x => !x.ExcludeRetainer && !x.Enabled && x.RetainerData.Count > 0);
            if(list.Any())
            {
                Warning($"部分角色未启用雇员多角色模式，但已登记雇员。悬停查看列表。", list.Print("\n"));
            }
        }
        {
            var list = C.OfflineData.Where(x => !x.ExcludeRetainer && x.Enabled && x.RetainerData.Count > 0 && C.SelectedRetainers.TryGetValue(x.CID, out var rd) && !x.RetainerData.All(r => rd.Contains(r.Name)));
            if(list.Any())
            {
                Warning($"部分角色未启用所有雇员进行处理。悬停查看列表。", list.Print("\n"));
            }
        }
        {
            var list = C.OfflineData.Where(x => !x.ExcludeWorkshop && !x.WorkshopEnabled && (x.OfflineSubmarineData.Count + x.OfflineAirshipData.Count) > 0);
            if(list.Any())
            {
                Warning($"部分角色未启用远航探索多角色模式，但已登记远航探索。悬停查看列表。", list.Print("\n"));
            }
        }

        {
            var list = C.OfflineData.Where(x => !x.ExcludeWorkshop && x.WorkshopEnabled && x.GetEnabledVesselsData(Internal.VoyageType.Airship).Count + x.GetEnabledVesselsData(Internal.VoyageType.Submersible).Count < Math.Min(x.OfflineAirshipData.Count + x.OfflineSubmarineData.Count, 4));
            if(list.Any())
            {
                Warning($"部分角色未启用所有远航探索进行处理。悬停查看列表。", list.Print("\n"));
            }
        }

        if(C.MultiModeType != AutoRetainerAPI.Configuration.MultiModeType.Everything)
        {
            Warning($"您的多角色模式类型设置为 {C.MultiModeType}。这将限制AutoRetainer执行的功能。");
        }

        if(C.OfflineData.Any(x => x.MultiWaitForAllDeployables))
        {
            Info("部分角色启用了\"等待所有待处理远航探索\"选项。悬停查看启用此选项的角色完整列表。", C.OfflineData.Where(x => x.MultiWaitForAllDeployables).Select(x => $"{x.Name}@{x.World}").Print("\n"));
        }

        if(C.MultiModeWorkshopConfiguration.MultiWaitForAll)
        {
            Info("全局选项\"等待派遣完成\"已启用。这意味着即使角色自身选项未启用，AutoRetainer也会等待所有远航探索返回后再处理该角色。");
        }

        if(C.MultiModeWorkshopConfiguration.WaitForAllLoggedIn)
        {
            Info("远航探索的\"即使已登录也等待\"选项已启用。这意味着即使您已登录角色，AutoRetainer也会等待该角色所有远航探索完成后才进行处理。");
        }

        if(C.DisableRetainerVesselReturn > 0)
        {
            if(C.DisableRetainerVesselReturn > 10)
            {
                Warning("选项\"雇员派遣处理截止时间\"设置值异常偏高。当远航探索即将可用时，重新派遣雇员可能会出现显著延迟。");
            }
            else
            {
                Info("选项\"雇员派遣处理截止时间\"已启用。当远航探索即将可用时，重新派遣雇员可能会出现延迟。");
            }
        }

        if(C.MultiModeRetainerConfiguration.MultiWaitForAll)
        {
            Info("选项\"等待派遣完成\"已启用。这意味着在登录角色处理雇员前，AutoRetainer会等待该角色所有雇员的派遣任务完成。");
        }

        if(C.MultiModeRetainerConfiguration.WaitForAllLoggedIn)
        {
            Info("雇员的\"即使已登录也等待\"选项已启用。这意味着即使您已登录角色，AutoRetainer也会等待该角色所有雇员的派遣任务完成后才进行处理。");
        }

        {
            var manualList = new List<string>();
            var deletedList = new List<string>();
            foreach(var x in C.OfflineData)
            {
                foreach(var ret in x.RetainerData)
                {
                    var planId = Utils.GetAdditionalData(x.CID, ret.Name).EntrustPlan;
                    var plan = C.EntrustPlans.FirstOrDefault(s => s.Guid == planId);
                    if(plan != null && plan.ManualPlan) manualList.Add($"{Censor.Character(x.Name)} - {Censor.Retainer(ret.Name)}");
                    if(plan == null && planId != Guid.Empty) deletedList.Add($"{Censor.Character(x.Name)} - {Censor.Retainer(ret.Name)}");
                }
            }
            if(manualList.Count > 0)
            {
                Info("部分雇员设置了手动托管计划。这些计划在重新派遣雇员后不会自动处理，仅在覆盖层点击按钮时手动执行。悬停查看列表。", manualList.Print("\n"));
            }
            if(deletedList.Count > 0)
            {
                Warning("部分雇员的托管计划已被删除。已删除托管计划的雇员将不会托管任何物品。悬停查看列表。", deletedList.Print("\n"));
            }
        }

        if(C.No2ndInstanceNotify)
        {
            Info("您已启用\"不警告从相同目录运行的第二个游戏实例\"选项，这将自动跳过在第二个游戏实例上加载AutoRetainer。");
        }

        if(Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == "SimpleTweaksPlugin" && x.IsLoaded))
        {
            Info("检测到Simple Tweaks插件。任何与雇员或潜艇相关的调整可能对AutoRetainer功能产生负面影响。请确保配置调整不会干扰AutoRetainer功能。");
        }

        if(Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == "PandorasBox" && x.IsLoaded))
        {
            Info("检测到Pandora's Box插件。在AutoRetainer启用时自动使用技能可能对功能产生负面影响。请确保配置Pandora's Box不会在AutoRetainer活动时自动使用技能。");
        }

        if(Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == "Automaton" && x.IsLoaded))
        {
            Info("检测到Automaton插件。在AutoRetainer启用时自动使用技能和自动输入数字可能对功能产生负面影响。请确保配置Automaton不会在AutoRetainer活动时自动使用技能。");
        }

        if(Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == "RotationSolver" && x.IsLoaded))
        {
            Info("检测到RotationSolver插件。在AutoRetainer启用时自动使用技能可能对功能产生负面影响。请确保配置RotationSolver不会在AutoRetainer活动时自动使用技能。");
        }

        if(Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName.StartsWith("BossMod") && x.IsLoaded))
        {
            Info("检测到BossMod插件。在AutoRetainer启用时自动使用技能可能对功能产生负面影响。请确保配置BossMod不会在AutoRetainer活动时自动使用技能。");
        }

        ImGui.Separator();
        ImGuiEx.TextWrapped("专家设置会改变开发者预期的行为。请检查您的问题是否与专家设置配置错误有关。");
        CheckExpertSetting("无可用派遣任务时访问雇员铃的操作", nameof(C.OpenBellBehaviorNoVentures));
        CheckExpertSetting("有可用派遣任务时访问雇员铃的操作", nameof(C.OpenBellBehaviorWithVentures));
        CheckExpertSetting("访问雇员铃后任务完成行为", nameof(C.TaskCompletedBehaviorAccess));
        CheckExpertSetting("手动启用后任务完成行为", nameof(C.TaskCompletedBehaviorManual));
        CheckExpertSetting("如果5分钟内雇员有派遣任务完成，则停留在雇员菜单", nameof(C.Stay5));
        CheckExpertSetting("关闭雇员列表时自动禁用插件", nameof(C.AutoDisable));
        CheckExpertSetting("不显示插件状态图标", nameof(C.HideOverlayIcons));
        CheckExpertSetting("显示多角色模式类型选择器", nameof(C.DisplayMMType));
        CheckExpertSetting("Display deployables checkbox in workshop", nameof(C.ShowDeployables));
        CheckExpertSetting("启用应急恢复模块", nameof(C.EnableBailout));
        CheckExpertSetting("AutoRetainer尝试解除卡死前的超时时间(秒)", nameof(C.BailoutTimeout));
        CheckExpertSetting("禁用排序和折叠/展开功能", nameof(C.NoCurrentCharaOnTop));
        CheckExpertSetting("在插件UI栏显示多角色模式复选框", nameof(C.MultiModeUIBar));
        CheckExpertSetting("雇员菜单延迟(秒)", nameof(C.RetainerMenuDelay));
        CheckExpertSetting("不检查派遣计划错误", nameof(C.NoErrorCheckPlanner2));
        CheckExpertSetting("启用多角色模式时，尝试进入附近房屋", nameof(C.MultiHETOnEnable));
        CheckExpertSetting("Artisan 整合功能", nameof(C.ArtisanIntegration));
        CheckExpertSetting("使用服务器时间而非本地时间", nameof(C.UseServerTime));
    }

    private static void Error(string message, string tooltip = null)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        ImGuiEx.Text(EColor.RedBright, "\uf057");
        ImGui.PopFont();
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
        ImGui.SameLine();
        ImGuiEx.TextWrapped(EColor.RedBright, message);
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
    }

    private static void Warning(string message, string tooltip = null)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        ImGuiEx.Text(EColor.OrangeBright, "\uf071");
        ImGui.PopFont();
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
        ImGui.SameLine();
        ImGuiEx.TextWrapped(EColor.OrangeBright, message);
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
    }

    private static void Info(string message, string tooltip = null)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        ImGuiEx.Text(EColor.YellowBright, "\uf05a");
        ImGui.PopFont();
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
        ImGui.SameLine();
        ImGuiEx.TextWrapped(EColor.YellowBright, message);
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
    }

    private static void CheckExpertSetting(string setting, string nameOfSetting)
    {
        var original = EmptyConfig.GetFoP(nameOfSetting);
        var current = C.GetFoP(nameOfSetting);
        if(!original.Equals(current))
        {
            Info($"专家设置 \"{setting}\" 与默认值不同", $"默认值为 \"{original}\", 当前值为 \"{current}\".");
        }
    }
}
