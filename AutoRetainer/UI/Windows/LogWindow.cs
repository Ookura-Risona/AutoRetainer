namespace AutoRetainer.UI.Windows;

internal class LogWindow : Window
{
    public LogWindow() : base("AutoRetainer 日志")
    {
        P.WindowSystem.AddWindow(this);
        SizeConstraints = new()
        {
            MinimumSize = new(200, 200),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };
    }

    public override void Draw()
    {
        InternalLog.PrintImgui();
    }
}
