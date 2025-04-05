using System.Drawing;
using FadeIn.Managers;
using FadeIn.Properties;
using MelonLoader;

namespace FadeIn.Utilities;

internal class Logger
{
    private readonly MelonLogger.Instance _logger = Melon<Main>.Logger;

    private static readonly Color LogColor = Color.FromArgb(
        MelonBuildInfo.ModColorAlpha,
        MelonBuildInfo.ModColorRed,
        MelonBuildInfo.ModColorGreen,
        MelonBuildInfo.ModColorBlue
    );

    internal Logger(string className)
    {
        _logger = new MelonLogger.Instance($"{MelonBuildInfo.ModName}.{className}", LogColor);
    }

    internal void Debug(object message)
    {
        if (!SettingsManager.Debug)
            return;
        _logger.Msg(message);
    }

    internal void Error(object message)
    {
        _logger.Error(message);
    }

    internal void Msg(object message)
    {
        _logger.Msg(message);
    }

    internal void Warning(object message)
    {
        _logger.Warning(message);
    }
}
