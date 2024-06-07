using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.InputOverlay
{
    internal class InputOverlayEntity : Entity
    {
        public InputOverlayEntity()
        {
            Tag = Tags.HUD;
        }

        readonly static float StartX = 30;
        static float StartY {
            get {
                return Settings.Instance.SpeedrunClock switch
                {
                    SpeedrunType.Chapter => 110,
                    SpeedrunType.File => 130,
                    _ => 30,
                };
            }
        }

        static float ButtonSize => InputOverlay.Settings.ButtonSize;
        static float ButtonPadX => InputOverlay.Settings.ButtonPaddingX;
        static float ButtonPadY => InputOverlay.Settings.ButtonPaddingY;

        static Color ButtonBG { get => new(InputOverlay.Settings.ButtonC, 0.5f); }
        static Color ButtonBGPressed { get => new(InputOverlay.Settings.ButtonPressedC, 0.2f); }
        static Color ButtonOutline { get => InputOverlay.Settings.ButtonOutlineC; }
        static Color TextColor { get => InputOverlay.Settings.TextC; }

        static Vector2 ButtonTextJustify { get; } = new(0.5f, 0.5f);
        static Vector2 ButtonTextScale { get => new(ButtonSize / 160f, ButtonSize / 160f); }

        static float ButtonTextOutlineSize { get; } = 1;
        static Color ButtonTextOutlineColor { get; } = Color.Black;

        public override void Render()
        {
            if (!InputOverlay.Settings.Enabled || (InputOverlay.IsPaused && InputOverlay.Settings.HideWhenPaused)) return;

            float x;
            float y = StartY;

            foreach (IReadOnlyList<InputOverlayButton> row in InputOverlay.Buttons)
            {
                x = StartX;
                foreach (InputOverlayButton btn in row)
                    DrawButton(btn, ref x, y);
                y += ButtonSize + ButtonPadY;
            }
        }

        static void DrawButton(InputOverlayButton btn, ref float x, float y)
        {
            Draw.Rect(x, y, ButtonSize, ButtonSize, btn.Value() ? ButtonBGPressed : ButtonBG);
            Draw.HollowRect(x, y, ButtonSize, ButtonSize, ButtonOutline);
            Vector2 pos = new(x + (ButtonSize / 2), y + (ButtonSize / 2));
            if (InputOverlay.Settings.TextOutline)
                ActiveFont.DrawOutline(btn.Text, pos, ButtonTextJustify, ButtonTextScale, TextColor, ButtonTextOutlineSize, ButtonTextOutlineColor);
            else
                ActiveFont.Draw(btn.Text, pos, ButtonTextJustify, ButtonTextScale, TextColor);
            x += ButtonSize + ButtonPadX;
        }
    }
}
