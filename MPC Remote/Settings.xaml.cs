using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace MPC_Remote
{
    public partial class Settings : PhoneApplicationPage
    {
        // Initalisierung der Setting Klasse
        private AppSettings AppSettings = new AppSettings();

        private List<ConnectionProfile> ConnectionList = new List<ConnectionProfile>();

        public Settings()
        {
            InitializeComponent();
            lB_AppSettings.ItemsSource = ConnectionList;
            Loaded += new RoutedEventHandler(Settings_Loaded);
        }

        void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            ConnectionList.Clear();
            ConnectionProfile AddProfile = new ConnectionProfile(); AddProfile.KeyName = "Add";
            ConnectionList.Add(AddProfile);
            foreach (string KeyName in AppSettings.Keys)
            {
                    if (KeyName.StartsWith("appsettings"))
                    {
                        ConnectionProfile TempConProfile = new ConnectionProfile();
                        TempConProfile.KeyName = KeyName;
                        TempConProfile.FromAppSettingsStringArray(AppSettings[KeyName]);
                        ConnectionList.Add(TempConProfile);
                    }
            }
            RefreshList();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RefreshList();
        }

        void tempGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                ContextMenu contextMenu = ContextMenuService.GetContextMenu(sender as Grid);
                contextMenu.IsOpen = true;
            }
            catch (Exception)
            {
            }
        }

        void SetDefaultMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (lB_AppSettings.SelectedItem != null)
            {
                ConnectionProfile SelectedProfile = lB_AppSettings.SelectedItem as ConnectionProfile;
                AppSettings.DefaultSetting = SelectedProfile.KeyName;
                RefreshList();
            }
        }

        void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (lB_AppSettings.SelectedItem != null)
            {
                ConnectionProfile SelectedProfile = lB_AppSettings.SelectedItem as ConnectionProfile;

                // Delete the Connection Profile
                ConnectionList.Remove(SelectedProfile);
                AppSettings.RemoveSetting(SelectedProfile.KeyName);

                // Check if setting which should be removed if DefaultSetting
                if (SelectedProfile.KeyName == AppSettings.DefaultSetting)
                {
                    if (ConnectionList.Count == 1)
                    {
                        AppSettings.DefaultSetting = "";
                    }
                    else
                    {
                        AppSettings.DefaultSetting = ConnectionList[1].KeyName;
                    }
                }
                RefreshList();
            }


        }

        void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (lB_AppSettings.SelectedItem != null)
            {
                ConnectionProfile SelectedProfile = lB_AppSettings.SelectedItem as ConnectionProfile;
                NavigationService.Navigate(new Uri("/AddSettings.xaml?edit=true" +
                                                                    "&KeyName=" + SelectedProfile.KeyName +
                                                                    "&Description=" + SelectedProfile.Description +
                                                                    "&Hostname=" + SelectedProfile.Hostname +
                                                                    "&Port=" + SelectedProfile.Port, UriKind.Relative));
            }
        }

        private void ApplicationBarWifiButton_Click(object sender, EventArgs e)
        {
            ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
            connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.WiFi;
            connectionSettingsTask.Show();
        }

        private void Grid_Add_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddSettings.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Sets current list of files as an item source into the ContentList
        /// </summary>
        private void RefreshList()
        {
            //Dispatcher.BeginInvoke(delegate()
            //{
                lB_AppSettings.ItemsSource = null;
                lB_AppSettings.ItemsSource = ConnectionList;
            //});
        }
    }

    public class GridTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AddGrid
        {
            get;
            set;
        }

        public DataTemplate SettingGrid
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ConnectionProfile _ConnectionProfile = item as ConnectionProfile;
            if (_ConnectionProfile != null)
            {
                if (_ConnectionProfile.KeyName == "Add")
                {
                    return AddGrid;
                }
                else
                {
                    return SettingGrid;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}