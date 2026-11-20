using Jellyfin.Plugin.FixResume.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.FixResume;

public class Plugin : BasePlugin<PluginConfiguration>
{
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    public static Plugin? Instance { get; private set; }

    public override string Name => "Fix Resume State";

    public override Guid Id => Guid.Parse("fe290b53-dc42-41f2-95f0-4b432168b3dd");

    public override string Description =>
        "Clears the resume position on items already marked Played.";
}