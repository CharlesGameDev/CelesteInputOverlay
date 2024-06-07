using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;
using CelesteSettings = Celeste.Settings;

namespace Celeste.Mod.InputOverlay;

public class InputOverlay : EverestModule {
    public static InputOverlay Instance { get; private set; }

    public override Type SettingsType => typeof(InputOverlaySettings);
    public static InputOverlaySettings Settings => (InputOverlaySettings) Instance._Settings;

    public static IReadOnlyList<IReadOnlyList<InputOverlayButton>> Buttons { get; private set; } = [
        [new("Jump", () => Input.Jump.Check), new("Up", () => Input.MoveY < 0), new("Grab", () => Input.Grab.Check)],
        [new("Left", () => Input.MoveX < 0), new("Down", () => Input.MoveY > 0), new("Right", () => Input.MoveX > 0)],
        [new("Dash", () => Input.Dash.Check), new("Demo", () => Input.CrouchDash.Check), new("Talk", () => Input.Talk.Check)]
    ];

    readonly static float StartX = 30;
    static float StartY
    {
        get
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
            || Fonts.loadedFonts.Count == 0
            || (Engine.Scene.Paused && Settings.HideWhenPaused)
            || (Settings.HideOutsideOfLevels && Engine.Scene is not Level)) return;
        Render();
    }

    private static void Render()
    {
        float x;
        float y = StartY;

        foreach (IReadOnlyList<InputOverlayButton> row in Buttons)
        {
            x = StartX;
            foreach (InputOverlayButton btn in row)
                DrawButton(btn, ref x, y);
            y += ButtonSize + ButtonPadY;
        }
    }

    static void DrawButton(InputOverlayButton btn, ref float x, float y)
    {
        Draw.SpriteBatch.Begin(0, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.ScreenMatrix);
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
        x += ButtonSize + ButtonPadX;
        Draw.SpriteBatch.End();
    }
}