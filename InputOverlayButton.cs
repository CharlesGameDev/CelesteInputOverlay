using System;

namespace Celeste.Mod.InputOverlay
{
    public readonly struct InputOverlayButton(string text, Func<bool> value)
    {
        public readonly string Text = text;
        public readonly Func<bool> Value = value;
    }
}
