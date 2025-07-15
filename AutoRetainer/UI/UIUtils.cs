﻿global using OverlayTextData = (System.Numerics.Vector2 Curpos, (bool Warning, string Text)[] Texts);
using AutoRetainerAPI.Configuration;
using ECommons.GameHelpers;
using ECommons.Interop;
using Lumina.Excel.Sheets;

namespace AutoRetainer.UI;

internal static class UIUtils
{
    public static bool PushColIfPreferredCurrent(this OfflineCharacterData data)
    {
        var normalColor = Player.CID == data.CID ? EColor.CyanBright : ImGui.GetStyle().Colors[(int)ImGuiCol.Text];
        if(data.Preferred)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, GradientColor.Get(normalColor, ImGuiColors.ParsedGreen));
            return true;
        }
        else
        {
            ImGui.PushStyleColor(ImGuiCol.Text, normalColor);
            return true;
        }
    }

    public static void DrawSearch()
    {
        if(!C.NoCharaSearch)
        {
            ImGuiEx.SetNextItemFullWidth();
            ImGui.InputTextWithHint("##search", "搜索角色...", ref Ref<string>.Get("SearchChara"), 50);
        }
    }

    public static void DrawDCV(this OfflineCharacterData data)
    {
        if(data.WorldOverride != null)
        {
            ImGui.PushFont(UiBuilder.IconFont);
            ImGuiEx.Text("\uf0ac");
            ImGui.PopFont();
            if(ImGuiEx.HoveredAndClicked("正在访问其他数据中心。右键点击清除此状态。", ImGuiMouseButton.Right))
            {
                data.WorldOverride = null;
            }
            ImGui.SameLine();
        }
    }

    public static void DrawTeleportIcons(ulong cid)
    {
        var offlineData = C.OfflineData.FirstOrDefault(x => x.CID == cid);
        if(offlineData == null) return;
        var data = S.LifestreamIPC.GetHousePathData(cid);
        if(offlineData.GetAllowFcTeleportForSubs() || offlineData.GetAllowFcTeleportForRetainers())
        {
            string error = null;
            if(data.FC == null)
            {
                error = "部队房屋未在Lifestream中注册";
            }
            else if(data.FC.PathToEntrance.Count == 0)
            {
                error = "部队房屋已在Lifestream注册，但未设置入口路径";
            }
            ImGui.PushFont(UiBuilder.IconFont);
            ImGuiEx.Text(error == null ? null : ImGuiColors.DalamudGrey3, "\uf1ad");
            ImGui.PopFont();
            ImGuiEx.Tooltip(error ?? $"部队房屋已在Lifestream注册且路径已设置。将会传送至部队房屋重新派遣远航探索。如启用，也将传送至部队房屋重新派遣雇员。\n地址: {Svc.Data.GetExcelSheet<Aetheryte>().GetRowOrDefault((uint)data.FC.ResidentialDistrict)?.Territory.Value.PlaceNameRegion.Value.Name}, {data.FC.Ward + 1}区, {data.FC.Plot + 1}号地");
            ImGui.SameLine(0, 3);
        }
        if(offlineData.GetAllowPrivateTeleportForRetainers())
        {
            string error = null;
            if(data.Private == null)
            {
                error = "私人房屋未在Lifestream中注册";
            }
            else if(data.Private.PathToEntrance.Count == 0)
            {
                error = "私人房屋已在Lifestream注册，但未设置入口路径";
            }
            ImGui.PushFont(UiBuilder.IconFont);
            ImGuiEx.Text(error == null ? null : ImGuiColors.DalamudGrey3, "\ue1b0");
            ImGui.PopFont();
            ImGuiEx.Tooltip(error ?? $"私人房屋已在Lifestream注册且路径已设置。将会传送至私人房屋重新派遣雇员。\n地址: {Svc.Data.GetExcelSheet<Aetheryte>().GetRowOrDefault((uint)data.Private.ResidentialDistrict)?.Territory.Value.PlaceNameRegion.Value.Name}, {data.Private.Ward + 1}区, {data.Private.Plot + 1}号地");
            ImGui.SameLine(0, 3);
        }
    }

    public static void DrawOverlayTexts(List<OverlayTextData> overlayTexts, ref float statusTextWidth)
    {
        if(overlayTexts.Count > 0)
        {
            var maxSizes = new float[overlayTexts[0].Texts.Length];
            for(var i = 0; i < maxSizes.Length; i++)
            {
                maxSizes[i] = overlayTexts.Select(x => ImGui.CalcTextSize(x.Texts[i].Text).X).Max();
            }
            foreach(var x in overlayTexts)
            {
                var cur = ImGui.GetCursorPos();
                for(var i = x.Texts.Length - 1; i >= 0; i--)
                {
                    var width = maxSizes[i..].Sum() + (maxSizes[i..].Length - 1) * ImGui.CalcTextSize("      ").X;
                    ImGui.SetCursorPos(new(x.Curpos.X - width, x.Curpos.Y));
                    if(statusTextWidth < width) statusTextWidth = width;
                    ImGuiEx.Text(x.Texts[i].Warning ? ImGuiColors.DalamudOrange : null, x.Texts[i].Text);
                }
                ImGui.SetCursorPos(cur);
            }
        }
    }

    public static float CollapsingHeaderSpacingsWidth => ImGui.GetStyle().FramePadding.X * 2f + ImGui.GetStyle().ItemSpacing.X * 2 + ImGui.CalcTextSize("▲...").X;

    public static string GetCutCharaString(this OfflineCharacterData data, float statusTextWidth)
    {
        var chstr = Censor.Character(data.Name, data.World);
        var mod = false;
        while(ImGui.CalcTextSize(chstr).X > ImGui.GetContentRegionAvail().X - statusTextWidth - UIUtils.CollapsingHeaderSpacingsWidth && chstr.Length > 5)
        {
            mod = true;
            chstr = chstr[0..^1];
        }
        if(mod) chstr += "...";
        return chstr;
    }

    internal static void SliderIntFrameTimeAsFPS(string name, ref int frameTime, int min = 1)
    {
        var fps = 60;
        if(frameTime != 0)
        {
            fps = GetFPSFromMSPT(frameTime);
        }
        ImGuiEx.SliderInt(name, ref fps, min, 60, fps == 60 ? "无限制" : null, ImGuiSliderFlags.AlwaysClamp);
        frameTime = fps == 60 ? 0 : (int)(1000f / fps);
    }

    public static int GetFPSFromMSPT(int frameTime)
    {
        return frameTime == 0 ? 60 : (int)(1000f / frameTime);
    }

    internal static void QRA(string text, ref LimitedKeys key)
    {
        if(DrawKeybind(text, ref key))
        {
            P.quickSellItems.Toggle();
        }
        ImGui.SameLine();
        ImGuiEx.Text("+ 右键点击");
    }

    private static string KeyInputActive = null;
    internal static bool DrawKeybind(string text, ref LimitedKeys key)
    {
        var ret = false;
        ImGui.PushID(text);
        ImGuiEx.Text($"{text}:");
        ImGui.Dummy(new(20, 1));
        ImGui.SameLine();
        ImGuiEx.SetNextItemWidthScaled(200f);
        if(ImGui.BeginCombo("##inputKey", $"{key}", ImGuiComboFlags.HeightLarge))
        {
            if(text == KeyInputActive)
            {
                ImGuiEx.Text(ImGuiColors.DalamudYellow, $"请按下新的按键...");
                foreach(var x in Enum.GetValues<LimitedKeys>())
                {
                    if(IsKeyPressed(x))
                    {
                        KeyInputActive = null;
                        key = x;
                        ret = true;
                        break;
                    }
                }
            }
            else
            {
                if(ImGui.Selectable("自动检测新按键", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    KeyInputActive = text;
                }
                ImGuiEx.Text($"手动选择按键:");
                ImGuiEx.SetNextItemFullWidth();
                ImGuiEx.EnumCombo("##selkeyman", ref key);
            }
            ImGui.EndCombo();
        }
        else
        {
            if(text == KeyInputActive)
            {
                KeyInputActive = null;
            }
        }
        if(key != LimitedKeys.None)
        {
            ImGui.SameLine();
            if(ImGuiEx.IconButton(FontAwesomeIcon.Trash))
            {
                key = LimitedKeys.None;
                ret = true;
            }
        }
        ImGui.PopID();
        return ret;
    }
}
