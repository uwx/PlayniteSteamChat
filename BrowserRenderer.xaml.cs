using System.Windows.Controls;
using Microsoft.Web.WebView2.Wpf;

namespace PlayniteSteamChat;

public partial class BrowserRenderer
{
    public BrowserRenderer(WebView2 webview)
    {
        InitializeComponent();
        BrowserControl.Content = webview;
    }
}