﻿using AutoRetainer.Modules.Statistics;
using ECommons.Configuration;
using Lumina.Excel.Sheets;
using System.IO;

namespace AutoRetainer.UI.Statistics;

public sealed class VentureStatsManager
{
    private VentureStatsManager() { }

    internal Dictionary<string, Dictionary<string, Dictionary<uint, StatisticsData>>> Data = [];
    internal Dictionary<string, uint> CharTotal = [];
    internal Dictionary<string, uint> RetTotal = [];
    internal Dictionary<(string Char, string Ret), HashSet<long>> VentureTimestamps = [];
    private string Filter = "";

    internal void DrawVentures()
    {
        if(Data.Count == 0)
        {
            Load();
        }
        if(ImGui.Button("重新加载"))
        {
            Load();
        }
        ImGui.SameLine();
        ImGui.Checkbox("合并显示高品质与普通品质物品", ref C.StatsUnifyHQ);
        ImGui.SameLine();
        ImGuiEx.SetNextItemFullWidth();
        ImGui.InputTextWithHint("##search", "筛选物品...", ref Filter, 100);
        var cindex = 0;
        foreach(var cData in Data)
        {
            var rindex = 0;
            var display = false;
            if(CharTotal[cData.Key] != 0)
            {
                if(ImGui.CollapsingHeader($"{Censor.Character(cData.Key)} | 总探险次数: {CharTotal.GetSafe(cData.Key)}###chara{cData.Key}"))
                {
                    display = true;
                }
            }
            CharTotal[cData.Key] = 0;
            foreach(var x in cData.Value)
            {
                var array = x.Value.Where(c => Filter == string.Empty || $"{Svc.Data.GetExcelSheet<Item>().GetRow(c.Key).Name}".Contains(Filter, StringComparison.OrdinalIgnoreCase));
                var num = (uint)GetVentureCount(cData.Key, x.Key);
                CharTotal[cData.Key] += num;
                if(display && num != 0)
                {
                    ImGui.Dummy(new(10, 1));
                    ImGui.SameLine();
                    if(ImGui.CollapsingHeader($"{Censor.Retainer(x.Key)} | 探险次数: {num}###{cData.Key}ret{x.Key}"))
                    {
                        foreach(var c in array)
                        {
                            var iName = $"{Svc.Data.GetExcelSheet<Item>().GetRow(c.Key).Name}";
                            ImGuiEx.Text($"             {iName}: {(C.StatsUnifyHQ ? c.Value.Amount + c.Value.AmountHQ : $"{c.Value.Amount}/{c.Value.AmountHQ}")}");
                        }
                    }
                }
            }
        }
    }

    internal void Load()
    {
        Data.Clear();
        VentureTimestamps.Clear();
        try
        {
            foreach(var x in Directory.GetFiles(Svc.PluginInterface.GetPluginConfigDirectory()))
            {
                if(x.EndsWith(".statistic.json"))
                {
                    var file = EzConfig.LoadConfiguration<StatisticsFile>(x);
                    foreach(var z in file.Records)
                    {
                        AddData(file.PlayerName, file.RetainerName, z.ItemId, z.IsHQ, z.Amount, z.Timestamp);
                    }
                }
            }
            foreach(var x in Data)
            {
                uint ctotal = 0;
                foreach(var z in x.Value)
                {
                    uint cnt = 0;
                    foreach(var c in z.Value.Values)
                    {
                        cnt += c.Amount + c.AmountHQ;
                    }
                    RetTotal[z.Key] = cnt;
                    ctotal += cnt;
                }
                CharTotal[x.Key] = ctotal;
            }
        }
        catch(Exception e)
        {
            e.Log();
            Notify.Error($"Error: {e.Message}");
        }
    }

    private int GetVentureCount(string character)
    {
        var ret = 0;
        foreach(var x in VentureTimestamps)
        {
            if(x.Key.Char == character)
            {
                ret += x.Value.Count;
            }
        }
        return ret;
    }

    private int GetVentureCount(string character, string retainer)
    {
        if(VentureTimestamps.TryGetValue((character, retainer), out var h))
        {
            return h.Count;
        }
        return 0;
    }

    private void AddData(string character, string retainer, uint item, bool hq, uint amount, long timestamp)
    {
        if(!Data.TryGetValue(character, out var cData))
        {
            cData = [];
            Data.Add(character, cData);
        }
        if(!cData.TryGetValue(retainer, out var rData))
        {
            rData = [];
            cData.Add(retainer, rData);
        }
        if(!rData.TryGetValue(item, out var iData))
        {
            iData = new();
            rData.Add(item, iData);
        }
        if(!VentureTimestamps.ContainsKey((character, retainer)))
        {
            VentureTimestamps[(character, retainer)] = [];
        }
        VentureTimestamps[(character, retainer)].Add(timestamp);
        if(hq)
        {
            iData.AmountHQ += amount;
        }
        else
        {
            iData.Amount += amount;
        }
    }
}
