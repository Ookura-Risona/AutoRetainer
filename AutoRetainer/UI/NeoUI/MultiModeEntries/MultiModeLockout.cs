using ECommons.ExcelServices;

namespace AutoRetainer.UI.NeoUI.MultiModeEntries;
public class MultiModeLockout : NeoUIEntry
{
    public override string Path => "多角色模式/区域锁定";

    private int Num = 12;

    public override void Draw()
    {
        ImGuiEx.TextV("在");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(150f);
        ImGui.InputInt("小时内...", ref Num.ValidateRange(1, 10000));
        foreach(var x in Enum.GetValues<ExcelWorldHelper.Region>())
        {
            if(ImGuiEx.IconButtonWithText(FontAwesomeIcon.Lock, $"不登录到 {x} 区域"))
            {
                C.LockoutTime[x] = DateTimeOffset.Now.ToUnixTimeSeconds() + Num * 60 * 60;
            }
        }
        if(ImGuiEx.IconButtonWithText(FontAwesomeIcon.Unlock, "移除所有锁定"))
        {
            C.LockoutTime.Clear();
        }
    }
}
