using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace SteamLikeLastActivity
{
    [PublicAPI]
    public class SteamLikeLastActivity : Plugin
    {
        // private static readonly ILogger Logger = LogManager.GetLogger();
        //
        // private SteamLikeLastActivitySettings Settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("5c4396d8-4be4-4efb-9db8-46cc83c263c0");

        public SteamLikeLastActivity(IPlayniteAPI api) : base(api)
        {
            // Settings = new SteamLikeLastActivitySettings(this);
            
            // Run first-time update process
            UpdateLastActivity(PlayniteApi.Database.Games);
            
            // Update newly added games
            api.Database.Games.ItemCollectionChanged += (sender, args) => UpdateLastActivity(args.AddedItems, true);
        }

        public override List<MainMenuItem> GetMainMenuItems(GetMainMenuItemsArgs args)
        {
            return new List<MainMenuItem>
            {
                new MainMenuItem
                {
                    MenuSection = "@",
                    Description = "Manually refresh Last Played",
                    Action = args1 => UpdateLastActivity(PlayniteApi.Database.Games)
                }
            };
        }

        private void UpdateLastActivity(IEnumerable<Game> gamesToUpdate, bool isBuffered = false)
        {
            if (!isBuffered) PlayniteApi.Database.Games.BeginBufferUpdate();

            foreach (var item in gamesToUpdate)
            {
                if (item.LastActivity == null)
                {
                    item.LastActivity = item.Added;
                    if (!isBuffered) PlayniteApi.Database.Games.Update(item);
                }
                else if (item.Added > item.LastActivity)
                {
                    item.LastActivity = item.Added;
                    if (!isBuffered) PlayniteApi.Database.Games.Update(item);
                }
            }

            if (!isBuffered) PlayniteApi.Database.Games.EndBufferUpdate();
        }

        // public override void OnGameInstalled(Game game)
        // {
        //     // Add code to be executed when game is finished installing.
        // }
        //
        // public override void OnGameStarted(Game game)
        // {
        //     // Add code to be executed when game is started running.
        // }
        //
        // public override void OnGameStarting(Game game)
        // {
        //     // Add code to be executed when game is preparing to be started.
        // }
        //
        // public override void OnGameStopped(Game game, long elapsedSeconds)
        // {
        //     // Add code to be executed when game is preparing to be started.
        // }
        //
        // public override void OnGameUninstalled(Game game)
        // {
        //     // Add code to be executed when game is uninstalled.
        // }
        //
        // public override void OnApplicationStarted()
        // {
        //     // Add code to be executed when Playnite is initialized.
        // }
        //
        // public override void OnApplicationStopped()
        // {
        //     // Add code to be executed when Playnite is shutting down.
        // }
        //
        // public override void OnLibraryUpdated()
        // {
        //     // Add code to be executed when library is updated.
        // }
        //
        // public override ISettings GetSettings(bool firstRunSettings)
        // {
        //     return settings;
        // }
        //
        // public override UserControl GetSettingsView(bool firstRunSettings)
        // {
        //     return new SteamLikeLastActivitySettingsView();
        // }
    }
}