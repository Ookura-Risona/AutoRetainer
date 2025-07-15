﻿using ECommons.EzIpcManager;

namespace AutoRetainer.Modules.EzIPCManagers;
public class IPC_GCContinuation
{
    public IPC_GCContinuation()
    {
        EzIPC.Init(this, $"{Svc.PluginInterface.InternalName}.GC");
    }

    [EzIPC]
    public void EnqueueInitiation()
    {
        GCContinuation.EnqueueInitiation(true);
    }

    [EzIPC]
    public GCInfo? GetGCInfo()
    {
        return GCContinuation.GetGCInfo();
    }
}
