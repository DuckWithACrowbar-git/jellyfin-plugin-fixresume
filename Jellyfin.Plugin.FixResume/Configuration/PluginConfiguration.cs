using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.FixResume.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    public int ClearThresholdPercent { get; set; } = 90;
}