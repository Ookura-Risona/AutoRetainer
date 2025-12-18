using AutoRetainerAPI.Configuration;
using Dalamud.Interface.Components;
using PunishLib.ImGuiMethods;

namespace AutoRetainer.UI.MainWindow;

internal static class SharedUI
{
    internal static void DrawLockout(OfflineCharacterData data)
    {
        if(data.IsLockedOut())
        {
            FontAwesome.PrintV(EColor.RedBright, FontAwesomeIcon.Lock);
            ImGuiEx.Tooltip("此角色位于您已临时禁用的数据中心。请前往配置界面取消禁用。");
            ImGui.SameLine();
        }
    }

    internal static void DrawMultiModeHeader(OfflineCharacterData data, string overrideTitle = null)
    {
        var b = true;
        ImGui.CollapsingHeader($"{Censor.Character(data.Name)} {overrideTitle ?? "配置"}##conf", ref b, ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Bullet | ImGuiTreeNodeFlags.OpenOnArrow);
        if(b == false)
        {
            ImGui.CloseCurrentPopup();
        }
        ImGui.Dummy(new(500, 1));
    }

    internal static void DrawServiceAccSelector(OfflineCharacterData data)
    {
        ImGuiEx.Text($"服务账号选择");
        ImGuiEx.SetNextItemWidthScaled(150);
        if(ImGui.BeginCombo("##Service Account Selection", $"服务账号 {data.ServiceAccount + 1}", ImGuiComboFlags.HeightLarge))
        {
            for(var i = 1; i <= 10; i++)
            {
                if(ImGui.Selectable($"服务账号 {i}"))
                {
                    data.ServiceAccount = i - 1;
                }
            }
            ImGui.EndCombo();
        }
    }

    internal static void DrawPreferredCharacterUI(OfflineCharacterData data)
    {
        if(ImGui.Checkbox("首选角色", ref data.Preferred))
        {
            foreach(var z in C.OfflineData)
            {
                if(z.CID != data.CID)
                {
                    z.Preferred = false;
                }
            }
        }
        ImGuiComponents.HelpMarker("在多角色模式下，当没有其他角色需要收取雇员时，插件会自动切换回您的首选角色。");
    }

    internal static void DrawExcludeReset(OfflineCharacterData data)
    {
        new NuiBuilder().Section("角色数据清除/重置", collapsible: true)
        .Widget(() =>
        {
            if(ImGuiEx.ButtonCtrl("排除角色"))
            {
                C.Blacklist.Add((data.CID, data.Name));
            }
            ImGuiComponents.HelpMarker("排除此角色将立即重置其设置，将其移出角色列表，并停止处理其所有雇员。您仍可手动操作此角色的雇员。可在设置中取消此操作。");
            if(ImGuiEx.ButtonCtrl("重置角色数据"))
            {
                new TickScheduler(() => C.OfflineData.RemoveAll(x => x.CID == data.CID));
            }
            ImGuiComponents.HelpMarker("角色保存的数据将被清除但不会排除该角色。当您再次登录此角色时，角色数据将重新生成。");

                if(ImGui.Button("Clear Free company data"))
            {
                data.ClearFCData();
            }
            ImGuiComponents.HelpMarker("Free company data, airships and submersibles will be removed from this character. Data will be regenerated once available.");
        }).Draw();
    }
}
