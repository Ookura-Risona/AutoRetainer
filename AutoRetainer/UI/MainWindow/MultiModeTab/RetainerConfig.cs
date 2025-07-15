using AutoRetainerAPI;
using AutoRetainerAPI.Configuration;

namespace AutoRetainer.UI.MainWindow.MultiModeTab;
public static unsafe class RetainerConfig
{
    public static void Draw(OfflineRetainerData ret, OfflineCharacterData data, AdditionalRetainerData adata)
    {
        ImGui.CollapsingHeader($"{Censor.Retainer(ret.Name)} - {Censor.Character(data.Name)} 配置  ##conf", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.OpenOnArrow);
        ImGuiEx.Text($"附加的探险后任务:");
        //ImGui.Checkbox($"Entrust Duplicates", ref adata.EntrustDuplicates);
        var selectedPlan = C.EntrustPlans.FirstOrDefault(x => x.Guid == adata.EntrustPlan);
        ImGuiEx.TextV($"委托物品:");
        if(!C.EnableEntrustManager) ImGuiEx.HelpMarker("已在设置中全局禁用", EColor.RedBright, FontAwesomeIcon.ExclamationTriangle.ToIconString());
        ImGui.SameLine();
        ImGui.SetNextItemWidth(150f);
        if(ImGui.BeginCombo($"##select", selectedPlan?.Name ?? "禁用", ImGuiComboFlags.HeightLarge))
        {
            if(ImGui.Selectable("禁用")) adata.EntrustPlan = Guid.Empty;
            for(var i = 0; i < C.EntrustPlans.Count; i++)
            {
                var plan = C.EntrustPlans[i];
                ImGui.PushID(plan.Guid.ToString());
                if(ImGui.Selectable(plan.Name, plan == selectedPlan))
                {
                    adata.EntrustPlan = plan.Guid;
                }
                ImGui.PopID();
            }
            ImGui.EndCombo();
        }
        if(ImGuiEx.IconButtonWithText(FontAwesomeIcon.Copy, "复制委托计划到..."))
        {
            ImGui.OpenPopup($"CopyEntrustPlanTo");
        }
        if(ImGui.BeginPopup("CopyEntrustPlanTo"))
        {
            if(ImGui.Selectable("复制到此角色的其他所有雇员"))
            {
                var cnt = 0;
                foreach(var x in data.RetainerData)
                {
                    cnt++;
                    Utils.GetAdditionalData(data.CID, x.Name).EntrustPlan = adata.EntrustPlan;
                }
                Notify.Info($"已更改 {cnt} 个雇员");
            }
            if(ImGui.Selectable("复制到此角色其他所有无委托计划的雇员"))
            {
                foreach(var x in data.RetainerData)
                {
                    var cnt = 0;
                    if(!C.EntrustPlans.Any(s => s.Guid == adata.EntrustPlan))
                    {
                        Utils.GetAdditionalData(data.CID, x.Name).EntrustPlan = adata.EntrustPlan;
                        cnt++;
                    }
                    Notify.Info($"已更改 {cnt} 个雇员");
                }
            }
            if(ImGui.Selectable("复制到所有角色的其他所有雇员"))
            {
                var cnt = 0;
                foreach(var offlineData in C.OfflineData)
                {
                    foreach(var x in offlineData.RetainerData)
                    {
                        Utils.GetAdditionalData(offlineData.CID, x.Name).EntrustPlan = adata.EntrustPlan;
                        cnt++;
                    }
                }
                Notify.Info($"已更改 {cnt} 个雇员");
            }
            if(ImGui.Selectable("复制到所有角色其他所有无委托计划的雇员"))
            {
                var cnt = 0;
                foreach(var offlineData in C.OfflineData)
                {
                    foreach(var x in offlineData.RetainerData)
                    {
                        var a = Utils.GetAdditionalData(data.CID, x.Name);
                        if(!C.EntrustPlans.Any(s => s.Guid == a.EntrustPlan))
                        {
                            a.EntrustPlan = adata.EntrustPlan;
                            cnt++;
                        }
                    }
                }
                Notify.Info($"已更改 {cnt} 个雇员");
            }
            ImGui.EndPopup();
        }
        ImGui.Checkbox($"存取金币", ref adata.WithdrawGil);
        if(adata.WithdrawGil)
        {
            if(ImGui.RadioButton("取出", !adata.Deposit)) adata.Deposit = false;
            if(ImGui.RadioButton("存入", adata.Deposit)) adata.Deposit = true;
            ImGuiEx.SetNextItemWidthScaled(200f);
            ImGui.InputInt($"百分比, %", ref adata.WithdrawGilPercent.ValidateRange(1, 100), 1, 10);
        }
        ImGui.Separator();
        Svc.PluginInterface.GetIpcProvider<ulong, string, object>(ApiConsts.OnRetainerSettingsDraw).SendMessage(data.CID, ret.Name);
        if(C.Verbose)
        {
            if(ImGui.Button("模拟就绪"))
            {
                ret.VentureEndsAt = 1;
            }
            if(ImGui.Button("模拟未就绪"))
            {
                ret.VentureEndsAt = P.Time + 60 * 60;
            }
        }
    }
}
