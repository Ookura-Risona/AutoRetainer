using AutoRetainerAPI.Configuration;
using Dalamud.Interface.Components;
using PunishLib.ImGuiMethods;

namespace AutoRetainer.UI.MainWindow.MultiModeTab;
public class CharaConfig
{
    public static void Draw(OfflineCharacterData data, bool isRetainer)
    {
        ImGui.PushID(data.CID.ToString());
        SharedUI.DrawMultiModeHeader(data);
        var b = new NuiBuilder()

        .Section("通用角色特定设置")
        .Widget(() =>
        {
            SharedUI.DrawServiceAccSelector(data);
            SharedUI.DrawPreferredCharacterUI(data);
        });
        if(isRetainer)
        {
            b = b.Section("雇员").Widget(() =>
            {
                ImGuiEx.Text($"自动筹备稀有品:");
                if(!AutoGCHandin.Operation)
                {
                    ImGuiEx.SetNextItemWidthScaled(200f);
                    ImGuiEx.EnumCombo("##gcHandin", ref data.GCDeliveryType);
                }
                else
                {
                    ImGuiEx.Text($"当前无法更改此设置");
                }
            });
        }
        else
        {
            b = b.Section("远航探索").Widget(() =>
            {
                ImGui.Checkbox($"等待所有待处理远航探索", ref data.MultiWaitForAllDeployables);
                ImGuiComponents.HelpMarker("在所有启用的远航探索返回前，阻止处理此角色");
            });
        }
        b = b.Section("传送覆盖设置", data.GetAreTeleportSettingsOverriden() ? ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg] with { X = 1f } : null, true)
        .Widget(() =>
        {
            ImGuiEx.Text($"您可以为每个角色覆盖传送设置");
            bool? demo = null;
            ImGuiEx.Checkbox("标记此图标的选项将使用全局配置中的值", ref demo);
            ImGuiEx.Checkbox("启用", ref data.TeleportOptionsOverride.Enabled);
            ImGui.Indent();
            ImGuiEx.Checkbox("为雇员传送...", ref data.TeleportOptionsOverride.Retainers);
            ImGui.Indent();
            ImGuiEx.Checkbox("...到私人房屋", ref data.TeleportOptionsOverride.RetainersPrivate);
            ImGuiEx.Checkbox("...到部队房屋", ref data.TeleportOptionsOverride.RetainersFC);
            ImGuiEx.Checkbox("...到公寓", ref data.TeleportOptionsOverride.RetainersApartment);
            ImGui.Text("如果以上所有选项都禁用或失败，将被传送到旅馆");
            ImGui.Unindent();
            ImGuiEx.Checkbox("为远航探索传送到部队房屋", ref data.TeleportOptionsOverride.Deployables);
            ImGui.Unindent();
            ImGuiGroup.EndGroupBox();
        }).Draw();
        SharedUI.DrawExcludeReset(data);
        ImGui.PopID();
    }
}
