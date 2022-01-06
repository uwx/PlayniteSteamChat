using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Markup;
using Newtonsoft.Json;
using Playnite.SDK;

namespace PlayniteSteamChat
{
    public class PlayniteSteamChatSettings : ISettings, INotifyPropertyChanged
    {
        public const int CurrentVersion = 1;

        private static readonly ILogger Logger = LogManager.GetLogger();
        
        private bool _stayOpen;
        public bool StayOpen
        {
            get => _stayOpen;
            set => OnPropertySet(ref _stayOpen, value);
        }

        private readonly PlayniteSteamChat _plugin;

        // Constructor for deserialization & wpf designer
        public PlayniteSteamChatSettings()
        {
        }

        public PlayniteSteamChatSettings(PlayniteSteamChat plugin) : this()
        {
            _plugin = plugin;
            var savedSettings = plugin.LoadPluginSettings<PlayniteSteamChatSettings>();
            if (savedSettings == null) return;

            StayOpen = savedSettings.StayOpen;
            // X = savedSettings.X;
        }

        public void BeginEdit()
        {
        }

        public void EndEdit()
        {
            _plugin.SavePluginSettings(this);
        }

        public void CancelEdit()
        {
        }

        public bool VerifySettings(out List<string> errors)
        {
            errors = new List<string>();
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected void OnPropertySet<T>(ref T oldValue, T newValue, [CallerMemberName] string property = null)
        {
            if (oldValue == null || newValue == null || !newValue.Equals(oldValue))
            {
                oldValue = newValue;
                OnPropertyChanged(property);
            }
        }

        protected void OnPropertySet<T>(ref T oldValue, T newValue, params string[] additionalProperties)
        {
            if (oldValue == null || newValue == null || !newValue.Equals(oldValue))
            {
                oldValue = newValue;
                foreach (var property in additionalProperties)
                {
                    OnPropertyChanged(property);
                }
            }
        }
    }
}