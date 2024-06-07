using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.InputOverlay;

public class InputOverlaySettings : EverestModuleSettings {

    public bool Enabled { get; set; } = true;

    public bool HideWhenPaused { get; set; } = false;
    public bool TextOutline { get; set; } = true;

    [SettingRange(0, 160, true), SettingSubText("Default: 50")]
    public int ButtonSize { get; set; } = 50;
    [SettingRange(0, 20), SettingSubText("Default: 5")]
    public int ButtonPaddingX { get; set; } = 5;
    [SettingRange(0, 20), SettingSubText("Default: 5")]
    public int ButtonPaddingY { get; set; } = 5;

    [SettingSubHeader("Colors (only changable on the main menu)")]
    [SettingSubText("Default: 000000")]
    public string ButtonColor { get => FormatColor(ButtonC); set => ButtonC = FormatColor(value); }
    [SettingSubText("Default: A52A2A")]
    public string ButtonPressedColor { get => FormatColor(ButtonPressedC); set => ButtonPressedC = FormatColor(value); }
    [SettingSubText("Default: 000000")]
    public string ButtonOutlineColor { get => FormatColor(ButtonOutlineC); set => ButtonOutlineC = FormatColor(value); }
    [SettingSubText("Default: FFFFFF")]
    public string TextColor { get => FormatColor(TextC); set => TextC = FormatColor(value); }

    public Color ButtonC = Color.Black;
    public Color ButtonPressedC = Color.Brown;
    public Color ButtonOutlineC = Color.Black;
    public Color TextC = Color.White;

    static string FormatColor(Color color) => $"{color.R:X2}{color.G:X2}{color.B:X2}";
    static Color FormatColor(string color) => Calc.HexToColor(color);
}