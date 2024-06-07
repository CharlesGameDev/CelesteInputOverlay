using Celeste.Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CelesteSettings = Celeste.Settings;

namespace Celeste.Mod.InputOverlay;

public class InputOverlay : EverestModule {
    public static InputOverlay Instance { get; private set; }

    public override Type SettingsType => typeof(InputOverlaySettings);
    public static InputOverlaySettings Settings => (InputOverlaySettings)Instance._Settings;

    public static IReadOnlyList<IReadOnlyList<InputOverlayButton>> Buttons { get; private set; } = [
        [new("Jump", () => Input.Jump.Check), new("Up", () => Input.MoveY < 0), new("Grab", () => Input.Grab.Check)],
        [new("Left", () => Input.MoveX < 0), new("Down", () => Input.MoveY > 0), new("Right", () => Input.MoveX > 0)],
        [new("Dash", () => Input.Dash.Check), new("Demo", () => Input.CrouchDash.Check), new("Talk", () => Input.Talk.Check)]
    ];

    public static readonly Dictionary<Corners, Corner> CornerPositions = new() {
        { Corners.TopLeft, new(() => 30, GetTopLeftCornerY, false, false) },
        { Corners.TopRight, new(() => Engine.Width - 100, () => 30, true, false) },
        { Corners.BottomLeft, new(() => 30, () => Engine.Height - 100, false, true) },
        { Corners.BottomRight, new(() => Engine.Width - 100, GetBottomRightCornerY, true, true) },
    };

    static float GetTopLeftCornerY()
    {
        if (Engine.Scene is not Level)
            return 30;
        return CelesteSettings.Instance.SpeedrunClock switch
        {
            SpeedrunType.Chapter => 110,
            SpeedrunType.File => 130,
            _ => 30,
        };
    }

    static float GetBottomRightCornerY()
    {
        if (Engine.Scene is MapEditor)
            return Engine.Height - 280;
        if (Engine.Scene is not Level && Engine.Scene is not AssetReloadHelper && Engine.Scene is not LevelExit)
            return Engine.Height - 180;
        return Engine.Height - 100;
    }

    static float ButtonSize => Settings.ButtonSize;
    static float ButtonPadX => Settings.ButtonPaddingX;
    static float ButtonPadY => Settings.ButtonPaddingY;

    static Color ButtonBG { get => new(Settings.ButtonC, 0.5f); }
    static Color ButtonBGPressed { get => new(Settings.ButtonPressedC, 0.2f); }
    static Color ButtonOutline { get => Settings.ButtonOutlineC; }
    static Color TextColor { get => Settings.TextC; }

    static Vector2 ButtonTextJustify { get; } = new(0.5f, 0.5f);
    static Vector2 ButtonTextScale { get => new(ButtonSize / 160f, ButtonSize / 160f); }

    static float ButtonTextOutlineSize { get; } = 1;
    static Color ButtonTextOutlineColor { get; } = Color.Black;

    public InputOverlay() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(InputOverlay), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(InputOverlayModule), LogLevel.Info);
#endif
    }

    public override void Load() {
        On.Monocle.Engine.RenderCore += Engine_RenderCore;
    }

    public override void Unload() {
        On.Monocle.Engine.RenderCore -= Engine_RenderCore;
    }

    private void Engine_RenderCore(On.Monocle.Engine.orig_RenderCore orig, Engine self)
    {
        orig(self);

        if (!Settings.Enabled
            || Engine.Scene is GameLoader
            || Engine.Scene is OverworldLoader
            || Engine.Scene is LevelLoader
            || Fonts.loadedFonts.Count == 0
            || (Engine.Scene.Paused && Settings.HideWhenPaused)
            || (Settings.HideOutsideOfLevels && Engine.Scene is not Level)) return;
        Render();
    }

    private static void Render()
    {
        Draw.SpriteBatch.Begin(0, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.ScreenMatrix);

        Corner corner = CornerPositions[Settings.Corner];
        float x;
        float y = corner.StartY();
        float xMul = corner.ReverseX ? -1 : 1;
        float yMul = corner.ReverseY ? -1 : 1;

        IEnumerable<IReadOnlyList<InputOverlayButton>> btns = Buttons;
        if (corner.ReverseY)
            btns = btns.Reverse();

        foreach (var _row in btns)
        {
            x = corner.StartX();
            IEnumerable<InputOverlayButton> row = _row;
            if (corner.ReverseX)
                row = row.Reverse();

            foreach (var btn in row)
            {
                DrawButton(btn, x, y);
                x += (ButtonSize + ButtonPadX) * xMul;
            }
            y += (ButtonSize + ButtonPadY) * yMul;
        }

        Draw.SpriteBatch.End();
    }

    static void DrawButton(InputOverlayButton btn, float x, float y)
    {
        Draw.Rect(x, y, ButtonSize, ButtonSize, btn.Value() ? ButtonBGPressed : ButtonBG);
        Draw.HollowRect(x, y, ButtonSize, ButtonSize, ButtonOutline);
        Vector2 pos = new(x + (ButtonSize / 2), y + (ButtonSize / 2));
        try
        {
            if (Settings.TextOutline)
                ActiveFont.DrawOutline(btn.Text, pos, ButtonTextJustify, ButtonTextScale, TextColor, ButtonTextOutlineSize, ButtonTextOutlineColor);
            else
                ActiveFont.Draw(btn.Text, pos, ButtonTextJustify, ButtonTextScale, TextColor);
        } catch (ArgumentOutOfRangeException) { }
    }
}