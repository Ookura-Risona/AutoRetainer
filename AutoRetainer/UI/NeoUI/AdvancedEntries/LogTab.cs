namespace AutoRetainer.UI.NeoUI.AdvancedEntries;
public class LogTab : NeoUIEntry
{
    public override string Path => "高级设置/日志";

    public override void Draw()
    {
        InternalLog.PrintImgui();
    }
}
