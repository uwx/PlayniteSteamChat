using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using CefSharp;
using CefSharp.Wpf;
using JetBrains.Annotations;

namespace PlayniteSteamChat
{
    [PublicAPI]
    public class PlayniteSteamChat : GenericPlugin
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        private static readonly IResourceProvider Resources = new ResourceProvider();

        public override Guid Id { get; } = Guid.Parse("6fcf7f1b-128f-4e9a-8854-54935be2ffba");

        private readonly SidebarItem[] _sidebarItems;
        private ChromiumWebBrowser _browser;
        private readonly PlayniteSteamChatSettings _settings;

        private static readonly string AssemblyFolder = Path.GetDirectoryName(typeof(PlayniteSteamChat).Assembly.Location) ?? throw new InvalidOperationException("GetDirectoryName was null");

        public PlayniteSteamChat(IPlayniteAPI api) : base(api)
        {
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
            
            //new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Noto Sans")
            _sidebarItems = new[]
            {
                new SidebarItem
                {
                    Type = SiderbarItemType.View,
                    Title = "Steam Chat",
                    Icon = new TextBlock
                    {
                        Text = "\uF1B6",
                        FontSize = 20,
                        FontFamily = new FontFamily(new Uri(Path.Combine(AssemblyFolder, "fontello.ttf")), "./#fontello")
                    },
                    Opened = GetOrCreateBrowser,
                    Closed = () =>
                    {
                        if (!_settings.StayOpen)
                        {
                            using var browser = _browser;
                            _browser = null;
                        }
                    }
                }
            };
            _settings = new PlayniteSteamChatSettings(this);
        }
        
        private Control GetOrCreateBrowser()
        {
            if (_browser != null)
            {
                return _browser;
            }
            
            Logger.Info("Initializing Chromium Browser");
            
            _browser = new ChromiumWebBrowser("https://steamcommunity.com/chat/");
            _browser.LoadingStateChanged += OnBrowserLoadingStateChanged;
            return _browser;
        }

        private void OnBrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            if (args.IsLoading)
            {
                return;
            }

            // Remove the load event handler, because we only want one snapshot of the initial page.
            _browser.LoadingStateChanged -= OnBrowserLoadingStateChanged;

            #if DEBUG
            _browser.ShowDevTools();
            #endif

            _ = Task.Run(async () =>
            {
                try
                {
                    await _browser.EvaluateScriptAsync(File.ReadAllText(Path.Combine(AssemblyFolder, "chatInitScript.js")));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "While evaluating chatInitScript");
                }
            });
        }

        public override IEnumerable<SidebarItem> GetSidebarItems() => _sidebarItems;

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

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return _settings;
        }

        public override UserControl GetSettingsView(bool firstRunView)
        {
            return new PlayniteSteamChatSettingsView();
        }
    }
}