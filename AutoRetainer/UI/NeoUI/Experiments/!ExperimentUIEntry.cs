﻿namespace AutoRetainer.UI.NeoUI.Experiments;
public abstract class ExperimentUIEntry : NeoUIEntry
{
    public virtual string Name => GetType().Name;
    public override string Path => $"实验性功能/{Name}";
}
