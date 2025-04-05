using System.Drawing;
using FadeIn.Managers;
using FadeIn.Properties;
using MelonLoader;

namespace FadeIn.Utilities;

internal class Logger
{
    internal static Logger Instance { get; } = new Logger();

    private readonly MelonLogger.Instance _logger;

    internal Logger(string name)
    {
        _logger = new MelonLogger.Instance(
            $"{MelonBuildInfo.ModName}.{name}",
            Color.FromArgb(
                MelonBuildInfo.ModColorAlpha,
                MelonBuildInfo.ModColorRed,
                MelonBuildInfo.ModColorGreen,
                MelonBuildInfo.ModColorBlue
            )
        );
    }

    internal Logger()
    {
        _logger = Melon<Main>.Logger;
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
