using System;

namespace Celeste.Mod.InputOverlay
{
    public readonly struct InputOverlayButton(string text, Func<bool> value)
    {
        public readonly string Text = text.Replace(" ", "\n");
        public readonly Func<bool> Value = value;
    }
}
