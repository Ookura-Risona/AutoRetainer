using ECommons.GameHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRetainer.UI.NeoUI;
public sealed unsafe class AccountWhitelist : NeoUIEntry
{
    public override void Draw()
    {
        ImGuiEx.TextWrapped($"您可以设置账户白名单。当您使用非白名单账户登录时，AutoRetainer将不会记录任何角色、雇员或潜水艇信息。");
        if(C.WhitelistedAccounts.Count == 0)
        {
            ImGuiEx.TextWrapped(EColor.GreenBright, "当前白名单状态：已禁用，如果需要启用，请添加账户到白名单。");
        }
        else
        {
            ImGuiEx.TextWrapped(EColor.YellowBright, "当前白名单状态：已启用，如果需要禁用，请移除所有账户。");
        }

        if(ImGuiEx.IconButtonWithText(FontAwesomeIcon.UserPlus, "添加当前账户", enabled: Player.Available))
        {
            C.WhitelistedAccounts.Add(*P.Memory.MyAccountId);
        }

        foreach(var x in C.WhitelistedAccounts)
        {
            ImGui.PushID(x.ToString());
            if(ImGuiEx.IconButton(FontAwesomeIcon.Trash))
            {
                new TickScheduler(() => C.WhitelistedAccounts.Remove(x));
            }
            ImGui.SameLine();
            ImGuiEx.TextV($"账户 {x}");
            ImGui.PopID();
        }
    }
}