using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRetainer.UI.NeoUI;
public unsafe sealed class UserInterface : NeoUIEntry
{
    public override string Path => "用户界面";

    public override NuiBuilder Builder => new NuiBuilder()

        .Section("用户界面")
        .Checkbox("匿名化雇员名称", () => ref C.NoNames, "雇员名称将在常规UI元素中被隐藏（调试菜单和插件日志中仍可见）。启用此选项后，不同插件界面中的角色和雇员编号可能不一致")
        .Checkbox("在雇员界面显示快捷菜单", () => ref C.UIBar)
        .Checkbox("显示扩展雇员信息", () => ref C.ShowAdditionalInfo, "在主界面显示雇员装等/获得力/鉴别力及其当前探险名称")
        .Widget("按ESC键时不关闭AutoRetainer窗口", (x) =>
        {
            if(ImGui.Checkbox(x, ref C.IgnoreEsc)) Utils.ResetEscIgnoreByWindows();
        })
        .Checkbox("在状态栏仅显示最高优先级图标", () => ref C.StatusBarMSI)
        .SliderInt(120f, "状态栏图标大小", () => ref C.StatusBarIconWidth, 32, 128)
        .Checkbox("游戏启动时打开AutoRetainer窗口", () => ref C.DisplayOnStart)
        .Checkbox("插件激活时跳过物品出售/交易确认", () => ref C.SkipItemConfirmations)
        .Checkbox("启用标题界面按钮（需重启插件）", () => ref C.UseTitleScreenButton)
        .Checkbox("隐藏角色搜索", () => ref C.NoCharaSearch)
        .Checkbox("不为已完成角色显示背景闪烁", () => ref C.NoGradient)
        .Checkbox("不警告同一目录运行多个游戏实例", () => ref C.No2ndInstanceNotify, "这将使AutoRetainer在第二个游戏实例中自动跳过加载，除非在主要实例中禁用此选项")

        .Section("雇员标签页角色排序")
        .Checkbox("启用排序", () => ref C.EnableRetainerSort)
        .TextWrapped("此排序仅影响视觉显示顺序，不会影响角色处理逻辑。")
        .Widget(() => UIUtils.DrawSortableEnumList("rorder", C.RetainersVisualOrders))

        .Section("远航探索标签页角色排序")
        .Checkbox("启用排序", () => ref C.EnableDeployablesSort)
        .TextWrapped("此排序仅影响视觉显示顺序，不会影响角色处理逻辑。")
        .Widget(() => UIUtils.DrawSortableEnumList("dorder", C.DeployablesVisualOrders));
}