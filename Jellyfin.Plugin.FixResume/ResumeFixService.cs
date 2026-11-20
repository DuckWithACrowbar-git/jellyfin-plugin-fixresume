using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.FixResume;

public class ResumeFixService : IHostedService
{
    private readonly IUserDataManager _userData;
    private readonly IUserManager _users;
    private readonly ILogger<ResumeFixService> _log;

    public ResumeFixService(IUserDataManager userData, IUserManager users,
                            ILogger<ResumeFixService> log)
    {
        _userData = userData;
        _users = users;
        _log = log;
    }

    public Task StartAsync(CancellationToken ct)
    {
        _userData.UserDataSaved += OnSaved;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken ct)
    {
        _userData.UserDataSaved -= OnSaved;
        return Task.CompletedTask;
    }

    private void OnSaved(object? sender, UserDataSaveEventArgs e)
    {

        _log.LogInformation(
            "UserDataSaved: {Item} reason={Reason} played={Played} ticks={Ticks} runtime={Runtime}",
            e.Item.Name, e.SaveReason, e.UserData.Played,
            e.UserData.PlaybackPositionTicks, e.Item.RunTimeTicks);

        var data = e.UserData;
        if (!data.Played || data.PlaybackPositionTicks <= 0)
        {
            return;
        }

        var threshold = Plugin.Instance?.Configuration.ClearThresholdPercent ?? 90;
        var runtime = e.Item.RunTimeTicks;

        if (runtime is > 0 &&
            data.PlaybackPositionTicks * 100.0 / runtime < threshold)
        {
            return;
        }

        _log.LogInformation("Clearing stale resume position on {Item}", e.Item.Name);
        data.PlaybackPositionTicks = 0;

        var user = _users.GetUserById(e.UserId);
        if (user is not null)
        {
            _userData.SaveUserData(user, e.Item, data,
                UserDataSaveReason.UpdateUserData, CancellationToken.None);
        }
    }
}