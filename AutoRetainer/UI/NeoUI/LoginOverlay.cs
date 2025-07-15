namespace AutoRetainer.UI.NeoUI;
public class LoginOverlay : NeoUIEntry
{
    public override string Path => "登录覆盖层";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
            .Section("登录覆盖层设置")
            .Checkbox("显示登录覆盖层", () => ref C.LoginOverlay)
            .Widget("登录覆盖层缩放比例", (x) =>
            {
                ImGuiEx.SetNextItemWidthScaled(150f);
                if(ImGuiEx.SliderFloat(x, ref C.LoginOverlayScale.ValidateRange(0.1f, 5f), 0.2f, 2f)) P.LoginOverlay.bWidth = 0;
            })
            .Widget($"登录覆盖层按钮内边距", (x) =>
            {
                ImGuiEx.SetNextItemWidthScaled(150f);
                if(ImGuiEx.SliderFloat(x, ref C.LoginOverlayBPadding.ValidateRange(0.5f, 5f), 1f, 1.5f)) P.LoginOverlay.bWidth = 0;
            })
        .Checkbox("搜索时显示隐藏角色", () => ref C.LoginOverlayAllSearch);
}
