using System;

namespace Celeste.Mod.InputOverlay
{
    public readonly struct Corner(Func<float> startX, Func<float> startY, bool reverseX, bool reverseY)
    {
        public readonly Func<float> StartX = startX;
        public readonly Func<float> StartY = startY;
        public readonly bool ReverseX = reverseX;
        public readonly bool ReverseY = reverseY;
    }

    public enum Corners
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    }
}
