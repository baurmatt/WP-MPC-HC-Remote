using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Coding4Fun.Phone.Controls.Toolkit;

namespace MPC_Remote
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Initalisierung der Setting Klasse
        private AppSettings AppSettings;

        // Initalisierung des ConnectionProfil Objects
        private ConnectionProfile ConnectionProfile;

        // Initalisiert den Timer für den kontinuierliche Abfruf der Status Daten
        System.Windows.Threading.DispatcherTimer GetStatusDataTimer = new System.Windows.Threading.DispatcherTimer();

        // Initalisierung der Status Variablen   
        Status _Status;

        // Konstruktor
        public MainPage()
        {
            // Initialisierung und Befüllen der GUI Objekte
            InitializeComponent();
            this.AppSettings = new AppSettings();
            this.ConnectionProfile = new ConnectionProfile();
            _Status = new Status();
            this.DataContext = _Status;

            // Datenkontext des Listenfeldsteuerelements auf die Beispieldaten festlegen
            //DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            // Deklariert und startet den Timer für kontinuierliche Abfruf der Status Daten
            this.GetStatusDataTimer.Interval = new TimeSpan(0, 0, 1);
            this.GetStatusDataTimer.Tick += new EventHandler(GetStatusDataTimer_Tick);
            this.GetStatusDataTimer.Start();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(!string.IsNullOrEmpty(this.AppSettings.DefaultSetting))
            {
                this.ConnectionProfile.KeyName = AppSettings.DefaultSetting;
                this.ConnectionProfile.FromAppSettingsStringArray(AppSettings[this.ConnectionProfile.KeyName]);
                this._Status = new Status();
                this.DataContext = _Status;
            }
            else
            {
                this.ConnectionProfile = new ConnectionProfile();
                this._Status = new Status();
                this.DataContext = _Status;
            }

            // Testet ob der MPC-HC schon läuft und fragt gegebenfalls Status Daten ab
            this._Status.GetStatusData(this.ConnectionProfile);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Set my connection profile for faster and easier debugging
            #if DEBUG
            if (string.IsNullOrEmpty(AppSettings.DefaultSetting))
            {
                ConnectionProfile tempConnectionProfile = new ConnectionProfile(AppSettings.GetNewAppSettingKeyName(), "baurmatt-PC", "192.168.1.3", "13579", "");
                this.AppSettings[tempConnectionProfile.KeyName] = tempConnectionProfile.ToStringArray();
                this.AppSettings.DefaultSetting = tempConnectionProfile.KeyName;
            }
            #endif

            if (this.AppSettings.FirstTimeStarted == false)
            {
                MessageBox.Show("This app requires a running remote instance of \'Media Player Classic - Home Cinema\' in Version \'1.5.2.3456 (x86 & x64)\' or greater.\nYou need to enable the build-in \'webinterface\' feature in \'MPC-HC\' to get this work. To do this open \'MPC-HC\' and go to:\n-> View -> Options... -> Web Interface.\nThere, you need to activate \'Listen on port:\' and save the settings.","MPC-HC Remote",MessageBoxButton.OK);
                this.AppSettings.FirstTimeStarted = true;
            }

            if (string.IsNullOrEmpty(this.ConnectionProfile.Hostname) || string.IsNullOrEmpty(this.ConnectionProfile.Port))
                MessageBox.Show("There are no settings set, please do this on the setting page!", "MPC-HC Remote", MessageBoxButton.OK);
        }

        #region Tick function for GetStatusDataTimer

        void GetStatusDataTimer_Tick(object sender, EventArgs e)
        {
            this._Status.GetStatusData(this.ConnectionProfile);
        }

        #endregion

        #region Control Page Buttons

        private void bt_PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (bt_PlayPauseImage.DataContext.ToString() == "appbar.transport.pause.rest.png")
            {
                CommandClient.ExecuteCommand(this.ConnectionProfile, MPCHCCommand.Pause);
            }
            else
            {
                CommandClient.ExecuteCommand(this.ConnectionProfile, MPCHCCommand.Play);
            }
            this._Status.GetStatusData(this.ConnectionProfile);
        }

        private void bt_VolumeMute_Click(object sender, RoutedEventArgs e)
        {
            CommandClient.ExecuteCommand(this.ConnectionProfile, MPCHCCommand.Mute);
        }

        private void bt_VolumeUp_Click(object sender, RoutedEventArgs e)
        {
            CommandClient.ExecuteCommand(this.ConnectionProfile, MPCHCCommand.VolumeUp);
        }

        private void bt_VolumeDown_Click(object sender, RoutedEventArgs e)
        {
            CommandClient.ExecuteCommand(this.ConnectionProfile, MPCHCCommand.VolumeDown);
        }

        private void bt_Forward_Click(object sender, RoutedEventArgs e)
        {
            CommandClient.ExecuteCommand(this.ConnectionProfile, MPCHCCommand.SkipForward);
            this._Status.GetStatusData(this.ConnectionProfile);
        }

        private void bt_Backward_Click(object sender, RoutedEventArgs e)
        {
            CommandClient.ExecuteCommand(this.ConnectionProfile, MPCHCCommand.SkipBackward);
            this._Status.GetStatusData(this.ConnectionProfile);
        }

        private void bt_Fullscreen_Click(object sender, RoutedEventArgs e)
        {
            CommandClient.ExecuteCommand(this.ConnectionProfile, MPCHCCommand.FullScreen);
        }

        private void bt_Fast_Backward_Click(object sender, RoutedEventArgs e)
        {
            CommandClient.ExecuteCommand(this.ConnectionProfile, MPCHCCommand.Slower);
        }

        private void bt_Fast_Forward_Click(object sender, RoutedEventArgs e)
        {
            CommandClient.ExecuteCommand(this.ConnectionProfile, MPCHCCommand.Faster);
        }

        #endregion

        #region ApplicationBar event handler
        private void ApplicationBarSettingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }
        private void ApplicationBarBrowseButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Browse.xaml", UriKind.Relative));
        }
        private void ApplicationBarMenuItem_info_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Info.xaml", UriKind.Relative));
        }
        #endregion

        #region TimeSpanPicker event handler
        private void tb_Timer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _TimeSpanPicker.ValueChanged -= _TimeSpanPicker_ValueChanged;
            TimeSpan TimeSpanDuration = TimeSpan.FromMilliseconds(_Status.duration);
            TimeSpan TimeSpanPosition = TimeSpan.FromMilliseconds(_Status.position);
            _TimeSpanPicker.Max = new TimeSpan(TimeSpanDuration.Hours, TimeSpanDuration.Minutes, TimeSpanDuration.Seconds);
            _TimeSpanPicker.Value = new TimeSpan(TimeSpanPosition.Hours, TimeSpanPosition.Minutes, TimeSpanPosition.Seconds);
            _TimeSpanPicker.Step = new TimeSpan(0, 0, 1);
            _TimeSpanPicker.ValueChanged += _TimeSpanPicker_ValueChanged;
            _TimeSpanPicker.OpenPicker();
        }

        void _TimeSpanPicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
        {
            CommandClient.ExecuteCommand(this.ConnectionProfile, MPCHCCommand.NewPosition + _TimeSpanPicker.Value.ToString());
            this._Status.GetStatusData(this.ConnectionProfile);
        }
        #endregion

        #region Slider event handler
        private void sl_PlayBackProgress_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            // Stops time, so user can slide undisturbed
            this.GetStatusDataTimer.Stop();
        }

        private void sl_PlayBackProgress_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            int value = Convert.ToInt32((sender as Slider).Value);
            CommandClient.ExecuteCommand(this.ConnectionProfile, MPCHCCommand.NewPosition + Status.GetValidTimeStringFromMilliseconds(value));
            this._Status.GetStatusData(this.ConnectionProfile);
            this.GetStatusDataTimer.Start();
        }

        private void sl_PlayBackProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this._Status.positionstring = Status.GetValidTimeStringFromMilliseconds(Convert.ToInt32((sender as Slider).Value));
        }
        #endregion
    }
}