using AutoRetainerAPI.Configuration;
using ECommons.MathHelpers;
using RetainerDescriptor = (ulong CID, string RetainerName);

namespace AutoRetainer.UI.NeoUI;
public class RetainersTab : NeoUIEntry
{
    public override string Path => "雇员管理";
    private int MassMinLevel = 0;
    private int MassMaxLevel = 100;
    private VenturePlan SelectedVenturePlan;
    private EntrustPlan SelectedEntrustPlan;
    private HashSet<RetainerDescriptor> SelectedRetainers = [];

    public override NuiBuilder Builder { get; init; }

    public RetainersTab()
    {
        Builder = new NuiBuilder()
                 .Section("批量配置更改")
                 .Widget(MassConfigurationChangeWidget);
    }

    private void MassConfigurationChangeWidget()
    {
        ImGuiEx.Text("选择雇员:");
        ImGuiEx.SetNextItemFullWidth();
        if(ImGui.BeginCombo("##sel", $"已选择 {SelectedRetainers.Count}", ImGuiComboFlags.HeightLarge))
        {
            ref var search = ref Ref<string>.Get("Search");
            ImGui.InputTextWithHint("##searchRetainers", "搜索角色", ref search, 100);
            foreach(var x in C.OfflineData)
            {
                if((search.Length > 0 && !(x.Name + "@" + x.World).Contains(search, StringComparison.OrdinalIgnoreCase)) || x.RetainerData.Count <= 0)
                {
                    continue;
                }
                ImGui.PushID(x.CID.ToString());
                ImGuiEx.CollectionCheckbox(Censor.Character(x.Name, x.World), x.RetainerData.Select(r => (x.CID, r.Name)), SelectedRetainers);
                ImGui.Indent();
                foreach(var r in x.RetainerData)
                {
                    ImGuiEx.CollectionCheckbox(Censor.Retainer(r.Name), (x.CID, r.Name), SelectedRetainers);
                }
                ImGui.Unindent();
                ImGui.PopID();
            }
            ImGui.EndCombo();
        }
        if(ImGuiEx.IconButtonWithText((FontAwesomeIcon)61527, "全部取消选择"))
        {
            SelectedRetainers.Clear();
        }
        ImGui.SameLine();
        if(ImGuiEx.IconButtonWithText((FontAwesomeIcon)61525, "全选所有雇员"))
        {
            SelectedRetainers.Clear();
            foreach(var x in C.OfflineData)
            {
                foreach(var v in x.RetainerData)
                {
                    SelectedRetainers.Add((x.CID, v.Name));
                }
            }
        }

        ImGui.Separator();

        ImGuiEx.TextV("按等级:");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(100f);
        ImGui.DragInt("##minlevel", ref MassMinLevel, 0.1f);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(100f);
        ImGui.DragInt("##maxlevel", ref MassMaxLevel, 0.1f);
        if(ImGuiEx.IconButtonWithText((FontAwesomeIcon)61543, "按等级添加雇员到选择"))
        {
            foreach(var x in C.OfflineData)
            {
                foreach(var r in x.RetainerData)
                {
                    if(r.Level.InRange(MassMinLevel, MassMaxLevel, includeEnd: true))
                    {
                        SelectedRetainers.Add((x.CID, r.Name));
                    }
                }
            }
        }

        ImGui.Separator();

        ImGuiEx.Text("操作:");
        ImGui.Separator();
        ImGui.SetNextItemWidth(150f);
        if(ImGui.BeginCombo("##ventureplans", SelectedVenturePlan?.Name ?? "未选择计划", (ImGuiComboFlags)8))
        {
            foreach(var plan in C.SavedPlans)
            {
                if(ImGui.Selectable(plan.Name + "##" + plan.GUID))
                {
                    SelectedVenturePlan = plan;
                }
            }
            ImGui.EndCombo();
        }
        ImGui.SameLine();
        if(ImGuiEx.IconButtonWithText((FontAwesomeIcon)62073, "启用探险计划"))
        {
            var num = 0;
            foreach(var x in SelectedRetainers)
            {
                var odata = C.OfflineData.FirstOrDefault(z => z.CID == x.CID);
                if (odata != null && SelectedVenturePlan != null)
                {
                    var adata = Utils.GetAdditionalData(x.CID, x.RetainerName);
                    adata.VenturePlan = SelectedVenturePlan;
                    //adata.VenturePlanIndex = (uint)(C.SavedPlans.IndexOf(SelectedVenturePlan) + 1);
                    adata.EnablePlanner = true;
                    num++;
                }
            }
            Notify.Success($"已应用于 {num} 个雇员");
        }

        ImGui.Separator();

        ImGui.SetNextItemWidth(150f);
        if(ImGui.BeginCombo("##entrustplans", SelectedEntrustPlan?.Name ?? "未选择计划", ImGuiComboFlags.HeightLarge))
        {
            foreach(var plan in C.EntrustPlans)
            {
                if(ImGui.Selectable($"{plan.Name}##{plan.Guid}"))
                {
                    SelectedEntrustPlan = plan;
                }
            }
            ImGui.EndCombo();
        }
        ImGui.SameLine();
        if(ImGuiEx.IconButtonWithText((FontAwesomeIcon)62566, "设置委托计划"))
        {
            var num = 0;
            foreach(var x in SelectedRetainers)
            {
                var odata = C.OfflineData.FirstOrDefault(z => z.CID == x.CID);
                if(odata != null)
                {
                    var adata = Utils.GetAdditionalData(x.CID, x.RetainerName);
                    adata.EntrustPlan = SelectedEntrustPlan.Guid;
                    num++;
                }
            }
            Notify.Success($"已应用于 {num} 个雇员");
        }

        ImGui.Separator();

        if(ImGuiEx.IconButtonWithText((FontAwesomeIcon)61526, "移除选定雇员的委托计划"))
        {
            var num = 0;
            foreach(var x in SelectedRetainers)
            {
                var odata = C.OfflineData.FirstOrDefault(z => z.CID == x.CID);
                if(odata != null)
                {
                    var adata = Utils.GetAdditionalData(x.CID, x.RetainerName);
                    adata.EntrustPlan = Guid.Empty;
                    num++;
                }
            }
            Notify.Success($"已应用于 {num} 个雇员");
        }

        ImGui.Separator();

        if(ImGuiEx.IconButtonWithText((FontAwesomeIcon)61526, "禁用选定雇员的探险计划"))
        {
            var num = 0;
            foreach(var x in SelectedRetainers)
            {
                var odata = C.OfflineData.FirstOrDefault(z => z.CID == x.CID);
                if(odata != null)
                {
                    var adata = Utils.GetAdditionalData(x.CID, x.RetainerName);
                    adata.EnablePlanner = false;
                    num++;
                }
            }
            Notify.Success($"已应用于 {num} 个雇员");
        }

        ImGui.Separator();

        if(ImGuiEx.IconButtonWithText((FontAwesomeIcon)61452, "启用选定雇员"))
        {
            var num = 0;
            foreach(var x in SelectedRetainers)
            {
                var retainers = P.GetSelectedRetainers(x.CID);
                retainers.Add(x.RetainerName);
                num++;
            }
            Notify.Success($"已应用于 {num} 个角色");
        }

        ImGui.Separator();

        if(ImGuiEx.IconButtonWithText((FontAwesomeIcon)61453, "禁用选定雇员"))
        {
            var num = 0;
            foreach(var x in SelectedRetainers)
            {
                var retainers = P.GetSelectedRetainers(x.CID);
                retainers.Remove(x.RetainerName);
                num++;
            }
            Notify.Success($"已应用于 {num} 个角色");
        }

        ImGui.Separator();

        if(ImGuiEx.IconButtonWithText((FontAwesomeIcon)61528, "启用选定雇员的多角色模式"))
        {
            var num = 0;
            foreach(var x in SelectedRetainers)
            {
                var odata = C.OfflineData.FirstOrDefault(z => z.CID == x.CID);
                if(odata is { Enabled: false })
                {
                    odata.Enabled = true;
                    num++;
                }
            }
            Notify.Success($"已应用于 {num} 个角色");
        }

        ImGui.Separator();

        if(ImGuiEx.IconButtonWithText((FontAwesomeIcon)61527, "禁用选定雇员的多人模式"))
        {
            var num = 0;
            foreach(var x in SelectedRetainers)
            {
                var odata = C.OfflineData.FirstOrDefault(z => z.CID == x.CID);
                if(odata is { Enabled: true })
                {
                    odata.Enabled = false;
                    num++;
                }
            }
            Notify.Success($"已应用于 {num} 个角色");
        }
    }
}
