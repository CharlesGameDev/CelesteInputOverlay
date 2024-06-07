using System;
using System.Collections.Generic;

namespace Celeste.Mod.InputOverlay;

public class InputOverlay : EverestModule {
    public static InputOverlay Instance { get; private set; }

    public override Type SettingsType => typeof(InputOverlaySettings);
    public static InputOverlaySettings Settings => (InputOverlaySettings) Instance._Settings;

    public static bool IsPaused { get; private set; }

    public static IReadOnlyList<IReadOnlyList<InputOverlayButton>> Buttons { get; private set; } = [
        [new("Jump", () => Input.Jump.Check), new("Up", () => Input.MoveY < 0), new("Grab", () => Input.Grab.Check)],
        [new("Left", () => Input.MoveX < 0), new("Down", () => Input.MoveY > 0), new("Right", () => Input.MoveX > 0)],
        [new("Dash", () => Input.Dash.Check), new("C. Dash", () => Input.CrouchDash.Check), new("Talk", () => Input.Talk.Check)]
    ];

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
        Everest.Events.Level.OnLoadLevel += Level_OnLoadLevel;
        Everest.Events.Level.OnPause += Level_OnPause;
        Everest.Events.Level.OnUnpause += Level_OnUnpause;
    }

    private void Level_OnUnpause(Level level)
    {
        IsPaused = false;
    }

    private void Level_OnPause(Level level, int startIndex, bool minimal, bool quickReset)
    {
        IsPaused = true;
    }

    private void Level_OnLoadLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader)
    {
        IsPaused = false;
        level.Add(new InputOverlayEntity());
    }

    public override void Unload() {
        Everest.Events.Level.OnLoadLevel -= Level_OnLoadLevel;
        Everest.Events.Level.OnPause -= Level_OnPause;
        Everest.Events.Level.OnUnpause -= Level_OnUnpause;
    }
}