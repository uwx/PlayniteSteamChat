using System.Windows;

namespace PlayniteSteamChat
{
    public partial class PlayniteSteamChatSettingsView
    {
        public PlayniteSteamChatSettingsView()
        {
            InitializeComponent();
        }

        private void OpenDevTools_OnClick(object sender, RoutedEventArgs e)
        {
            ((PlayniteSteamChatSettings)DataContext)?.OpenDevTools();
        }
    }
}