using Playnite.SDK;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using JetBrains.Annotations;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace PlayniteSteamChat
{
    [PublicAPI]
    public class PlayniteSteamChat : GenericPlugin
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        private static readonly IResourceProvider Resources = new ResourceProvider();

        public override Guid Id { get; } = Guid.Parse("6fcf7f1b-128f-4e9a-8854-54935be2ffba");

        private readonly SidebarItem[] _sidebarItems;
        private WebView2 _browser;
        private BrowserRenderer _browserControl;
        private readonly PlayniteSteamChatSettings _settings;

        private static readonly string AssemblyFolder = Path.GetDirectoryName(typeof(PlayniteSteamChat).Assembly.Location) ?? throw new InvalidOperationException("GetDirectoryName was null");

        private readonly Lazy<Task<CoreWebView2Environment>> _webViewEnvironment;
        private Task<CoreWebView2Environment> WebViewEnvironment => _webViewEnvironment.Value;

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
                            _browserControl = null;
                        }
                    }
                }
            };
            _settings = new PlayniteSteamChatSettings(this);

            _webViewEnvironment = new Lazy<Task<CoreWebView2Environment>>(() => CoreWebView2Environment.CreateAsync(userDataFolder: GetPluginUserDataPath()));
        }
        
        private Control GetOrCreateBrowser()
        {
            if (_browserControl != null)
            {
                return _browserControl;
            }
            
            Logger.Info("Initializing Chromium Browser");

            _browser = new WebView2();
            _ = HandleWebViewInitAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Logger.Error(task.Exception, "WebView initialization failed");
                }
            });

            _browserControl = new BrowserRenderer(_browser);
            return _browserControl;
        }

        private async Task HandleWebViewInitAsync()
        {
            await _browser.EnsureCoreWebView2Async(await WebViewEnvironment);
            await _browser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(File.ReadAllText(Path.Combine(AssemblyFolder, "chatInitScript.js")));
            _browser.CoreWebView2.Navigate("https://steamcommunity.com/chat/");

            #if DEBUG
            _browser.CoreWebView2.OpenDevToolsWindow();
            #endif
        }

        internal async Task OpenDevToolsAsync()
        {
            await _browser.EnsureCoreWebView2Async(await WebViewEnvironment);
            _browser.CoreWebView2.OpenDevToolsWindow();
        }

        public override IEnumerable<SidebarItem> GetSidebarItems() => _sidebarItems;

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