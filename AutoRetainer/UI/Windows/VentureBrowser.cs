﻿using AutoRetainerAPI.Configuration;
using ECommons.ExcelServices;
using ECommons.GameHelpers;

namespace AutoRetainer.UI.Windows;

internal class VentureBrowser : Window
{
    private OfflineRetainerData SelectedRetainer = null;
    private OfflineCharacterData SelectedCharacter = null;
    private List<VentureBrowserData> Data = [];
    private int MaxGathering = 0;
    private int MaxPerception = 0;
    private int MaxIlvl = 0;
    private string search = "";
    private int minLevel = 1;
    private int maxLevel = Player.MaxLevel;
    private bool GatherBuddyPresent = false;
    public VentureBrowser() : base("探险委托浏览器")
    {
        P.WindowSystem.AddWindow(this);
        SizeConstraints = new()
        {
            MinimumSize = new(100, 100),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };
    }

    internal void Reset()
    {
        Data.Clear();
        MaxGathering = 0;
        MaxPerception = 0;
        MaxIlvl = 0;
    }

    public override void OnClose()
    {
        Reset();
    }

    public override void OnOpen()
    {
        GatherBuddyPresent = Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == "GatherBuddy");
    }

    public override void Draw()
    {
        ImGuiEx.SetNextItemFullWidth();
        if(ImGui.BeginCombo("##selectRet", SelectedCharacter != null ? $"{Censor.Character(SelectedCharacter.Name, SelectedCharacter.World)} - {Censor.Retainer(SelectedRetainer.Name)} - {SelectedRetainer.Level} {ExcelJobHelper.GetJobNameById(SelectedRetainer.Job)}" : "选择雇员...", ImGuiComboFlags.HeightLarge))
        {
            foreach(var x in C.OfflineData.OrderBy(x => !C.NoCurrentCharaOnTop && x.CID == Player.CID ? 0 : 1))
            {
                foreach(var r in x.RetainerData)
                {
                    if(ImGui.Selectable($"{Censor.Character(x.Name, x.World)} - {Censor.Retainer(r.Name)} - Lv{r.Level} {ExcelJobHelper.GetJobNameById(r.Job)}"))
                    {
                        SelectedRetainer = r;
                        SelectedCharacter = x;
                        Reset();
                    }
                }
            }
            ImGui.EndCombo();
        }

        if(SelectedRetainer != null && SelectedCharacter != null)
        {
            var adata = Utils.GetAdditionalData(SelectedCharacter.CID, SelectedRetainer.Name);
            if(VentureUtils.IsDoL(SelectedRetainer.Job))
            {
                ImGuiEx.TextCentered($"{Lang.CharLevel}{SelectedRetainer.Level} {ExcelJobHelper.GetJobNameById(SelectedRetainer.Job)} | 获得力: {adata.Gathering} ({adata.Gathering / (float)MaxGathering:P0}) | 鉴别力: {adata.Perception} ({adata.Perception / (float)MaxPerception:P0})");
            }
            else
            {
                ImGuiEx.TextCentered($"{Lang.CharLevel}{SelectedRetainer.Level} {ExcelJobHelper.GetJobNameById(SelectedRetainer.Job)} | 装等: {adata.Ilvl} ({adata.Ilvl / (float)MaxPerception:P0})");
            }
            ImGuiEx.InputWithRightButtonsArea("VBrowser", delegate
            {
                ImGui.InputTextWithHint("##search", "过滤...", ref search, 100);
            }, delegate
            {
                ImGuiEx.TextV($"{Lang.CharLevel}:");
                ImGui.SameLine();
                ImGuiEx.SetNextItemWidthScaled(50f);
                ImGui.DragInt("##minL", ref minLevel, 1, 1, Player.MaxLevel);
                ImGui.SameLine();
                ImGuiEx.Text($"-");
                ImGui.SameLine();
                ImGuiEx.SetNextItemWidthScaled(50f);
                ImGui.DragInt("##maxL", ref maxLevel, 1, 1, Player.MaxLevel);
            });
            if(adata.Gathering == -1 || adata.Perception == -1 || adata.Ilvl == -1 || SelectedRetainer.Level == 0)
            {
                ImGuiEx.TextWrapped($"该雇员缺少数据。请访问雇员铃并选择该雇员以填充数据。");
            }
            else
            {
                if(Data.Count == 0)
                {
                    foreach(var v in VentureUtils.GetHunts(SelectedRetainer.Job))
                    {
                        var parts = v.GetFancyVentureNameParts(SelectedCharacter, SelectedRetainer, out var avail);
                        VentureBrowserData ventureBrowserData = new()
                        {
                            VentureName = parts.Name,
                            VentureLevel = v.RetainerLevel,
                            AvailableByGear = VentureUtils.IsDoL(SelectedRetainer.Job) ? adata.Gathering >= v.RequiredGathering : adata.Ilvl >= v.RequiredItemLevel,
                            Gathered = SelectedCharacter.UnlockedGatheringItems.Contains(VentureUtils.GetGatheringItemByItemID(v.GetVentureItemId())),
                            CurrentIndex = parts.YieldRate,
                            IsDol = VentureUtils.IsDoL(SelectedRetainer.Job),
                            IlvlGathering = VentureUtils.IsDoL(SelectedRetainer.Job) ? v.RequiredGathering : v.RequiredItemLevel,
                            Available = avail,
                            ID = v.RowId,
                            ItemID = v.GetVentureItemId()
                        };
                        (ventureBrowserData.Requirements, ventureBrowserData.Amounts) = v.GetVentureAmounts(SelectedRetainer);
                        if(v.RequiredGathering > MaxGathering) MaxGathering = v.RequiredGathering;
                        if(v.RequiredItemLevel > MaxIlvl) MaxIlvl = v.RequiredItemLevel;
                        if(ventureBrowserData.Requirements[4] > MaxPerception) MaxPerception = ventureBrowserData.Requirements[4];
                        Data.Add(ventureBrowserData);
                    }
                }
                if(ImGui.BeginTable("##VentureBrowserTable", 9, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY))
                {
                    ImGui.TableSetupScrollFreeze(0, 1);
                    ImGui.TableSetupColumn(Lang.CharLevel);
                    ImGui.TableSetupColumn("名称", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn(Data.FirstOrDefault()?.IsDol == true ? Lang.CharPlant : Lang.CharItemLevel);
                    ImGui.TableSetupColumn("☆☆☆☆");
                    ImGui.TableSetupColumn("★☆☆☆");
                    ImGui.TableSetupColumn("★★☆☆");
                    ImGui.TableSetupColumn("★★★☆");
                    ImGui.TableSetupColumn("★★★★");
                    ImGui.TableSetupColumn("已解锁");
                    ImGui.TableHeadersRow();

                    foreach(var x in Data.Where(x => x.VentureName.Contains(search, StringComparison.OrdinalIgnoreCase) && x.VentureLevel >= minLevel && x.VentureLevel <= maxLevel))
                    {
                        var n = x.IsDol ? Lang.CharP : Lang.CharItemLevel;
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGuiEx.TextCentered(SelectedRetainer.Level >= x.VentureLevel ? ImGuiColors.ParsedGreen : ImGuiColors.DalamudRed, $"{x.VentureLevel}");
                        ImGui.TableNextColumn();
                        ImGuiEx.Text($"{x.VentureName}");
                        if(ImGui.SmallButton($"添加到计划##{x.ID}"))
                        {
                            adata.VenturePlan.List.Add(new(x.ID));
                        }
                        ImGui.SameLine();
                        if(ImGui.SmallButton($"查询价格##{x.ID}"))
                        {
                            Svc.Commands.ProcessCommand($"/pmb {x.ItemID}");
                        }
                        ImGui.TableNextColumn();
                        ImGuiEx.TextCentered(x.AvailableByGear ? ImGuiColors.ParsedGreen : ImGuiColors.DalamudRed, $"{x.IlvlGathering}");
                        ImGui.TableNextColumn();

                        Vector4? col = x.AvailableByGear && x.CurrentIndex == 0 ? ImGuiColors.ParsedGreen : null;
                        ImGuiEx.TextCentered(col, $"  {n} < {x.Requirements[1]}  ");
                        ImGuiEx.TextCentered(col, $"  {x.Amounts[0].FancyDigits()}  ");
                        ImGui.TableNextColumn();

                        col = x.AvailableByGear && x.CurrentIndex == 1 ? ImGuiColors.ParsedGreen : null;
                        ImGuiEx.TextCentered(col, $"  {n} {x.Requirements[1]} - {x.Requirements[2] - 1}  ");
                        ImGuiEx.TextCentered(col, $"  {x.Amounts[1].FancyDigits()}  ");
                        ImGui.TableNextColumn();

                        col = x.AvailableByGear && x.CurrentIndex == 2 ? ImGuiColors.ParsedGreen : null;
                        ImGuiEx.TextCentered(col, $"  {n} {x.Requirements[2]} - {x.Requirements[3] - 1}  ");
                        ImGuiEx.TextCentered(col, $"  {x.Amounts[2].FancyDigits()}  ");
                        ImGui.TableNextColumn();

                        col = x.AvailableByGear && x.CurrentIndex == 3 ? ImGuiColors.ParsedGreen : null;
                        ImGuiEx.TextCentered(col, $"  {n} {x.Requirements[3]} - {x.Requirements[4] - 1}  ");
                        ImGuiEx.TextCentered(col, $"  {x.Amounts[3].FancyDigits()}  ");
                        ImGui.TableNextColumn();

                        col = x.AvailableByGear && x.CurrentIndex == 4 ? ImGuiColors.ParsedGreen : null;
                        ImGuiEx.TextCentered(col, $"{n} {x.Requirements[4]}+");
                        ImGuiEx.TextCentered(col, $"  {x.Amounts[4].FancyDigits()}  ");
                        ImGui.TableNextColumn();

                        if(x.IsDol)
                        {
                            ImGuiEx.Text(x.Gathered ? ImGuiColors.ParsedGreen : ImGuiColors.DalamudRed, x.Gathered ? "是" : "否");
                            if(!x.Gathered && GatherBuddyPresent)
                            {
                                if(ImGui.SmallButton($"采集##{x.ID}"))
                                {
                                    Svc.Commands.ProcessCommand($"/gather {x.VentureName}");
                                }
                            }
                        }
                        else
                        {
                            ImGuiEx.Text($"常驻");
                        }
                    }


                    ImGui.EndTable();
                }
            }
        }
    }

    internal class VentureBrowserData
    {
        internal string VentureName;
        internal int[] Requirements = new int[5];
        internal int[] Amounts = new int[5];
        internal int CurrentIndex;
        internal bool Gathered;
        internal bool AvailableByGear;
        internal int VentureLevel;
        internal bool IsDol;
        internal int IlvlGathering;
        internal bool Available;
        internal uint ID;
        internal uint ItemID;
    }
}
