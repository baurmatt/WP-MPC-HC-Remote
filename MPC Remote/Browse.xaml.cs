using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Text.RegularExpressions;

namespace MPC_Remote
{
    public partial class Browse : PhoneApplicationPage
    {
        // Initalisierung der Setting Klasse
        private AppSettings AppSettings = new AppSettings();

        // Initalisierung des ConnectionProfil Objects
        private ConnectionProfile ConnectionProfile = new ConnectionProfile();

        // Initalisierung der BrowseItemsList, Liste für Browse Übersicht
        List<BrowseItem> BrowseItemsList = new List<BrowseItem>();

        string PathBack;

        ProgressBar loadingbar = new ProgressBar();

        TextBlock ErrorTextBlock = new TextBlock();

        public Browse()
        {
            InitializeComponent();
            ErrorTextBlock.Height = 138;
            ErrorTextBlock.Width = 327;
            ErrorTextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            ErrorTextBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            ErrorTextBlock.Margin = new Thickness(59,114,0,0);
            ErrorTextBlock.Name = "ErrorTextBlock";
            ErrorTextBlock.FontWeight = FontWeights.Bold;
            ErrorTextBlock.TextAlignment = TextAlignment.Center;
            ErrorTextBlock.TextWrapping = TextWrapping.Wrap;
            ErrorTextBlock.Text = "Browse content could not be retrieved! Please check your connection and network settings! Also make sure your remote instance of MPC-HC is running!";
            this.Loaded += new RoutedEventHandler(Browse_Loaded);
        }

        void Browse_Loaded(object sender, RoutedEventArgs e)
        {
            // Wenn noch kein ConnectionProfile deklariert wurde oder sich das ausgewählte Setting Profile geändert hat ABER nur wenn es überhaupt ein ausgewähltes Setting Profile gibt.
            if ((string.IsNullOrEmpty(ConnectionProfile.KeyName) || ConnectionProfile.KeyName != AppSettings.DefaultSetting) && !string.IsNullOrEmpty(AppSettings.DefaultSetting))
            {
                ConnectionProfile.KeyName = AppSettings.DefaultSetting;
                ConnectionProfile.FromAppSettingsStringArray(AppSettings[ConnectionProfile.KeyName]);
            }

            if (LB_BrowseItems.ItemsSource == null)
                GetBrowseItems(ConnectionProfile.BrowsePath);
        }

        #region Button methods
        private void tb_BrowseItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string Path = ((BrowseItem)((TextBlock)sender).DataContext).Path;
            GetBrowseItems(Path);
        }

        private void tb_BrowseItem_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string Name = ((BrowseItem)((TextBlock)sender).DataContext).Name;
            MessageBox.Show(Name);
        }

        private void ApplicationBarBackButton_Click(object sender, EventArgs e)
        {
            GetBrowseItems(PathBack);
        }

        private void ApplicationBarHomeButton_Click(object sender, EventArgs e)
        {
            GetBrowseItems();
        }
        #endregion

        #region GetBrowseItems methods
        void GetBrowseItems(string Path = "")
        {
            // Checks if Hostname or Port are emty
            if (string.IsNullOrEmpty(AppSettings.DefaultSetting))
            {
                BrowseItemsList.Clear();
                RefreshListBox();
                ErrorTextBlock.Text = "There are no settings set, please do this on the setting page!";
                if (!this.ContentPanel.Children.Contains(ErrorTextBlock))
                    this.ContentPanel.Children.Add(ErrorTextBlock);
                return;
            }
            else
            {
                if (this.ContentPanel.Children.Contains(ErrorTextBlock))
                    this.ContentPanel.Children.Remove(ErrorTextBlock);
            }

            ActivateProgressBar();

            if (!CommandClient.IsConnectionAvailable(ConnectionProfile.Hostname, Convert.ToInt32(ConnectionProfile.Port)))
            {
                BrowseItemsList.Clear();
                RefreshListBox();
                ErrorTextBlock.Text = "Browse content could not be retrieved! Please check your connection and network settings! Also make sure your remote instance of MPC-HC is running!";
                if (!this.ContentPanel.Children.Contains(ErrorTextBlock))
                    this.ContentPanel.Children.Add(ErrorTextBlock);
                DeactivateProgressBar();
                return;
            }
            else
            {
                if (this.ContentPanel.Children.Contains(ErrorTextBlock))
                    this.ContentPanel.Children.Remove(ErrorTextBlock);
            }

            string requestUrl = @"http://" + ConnectionProfile.Hostname + ":" + ConnectionProfile.Port + "/browser.html?path=" + Path;
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri(requestUrl), UriKind.Absolute);
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                BrowseItemsList.Clear();

                // Getting File Name, Path and Size
                string pattern = ".*path=(?<Path>.*?)\">(?<Name>.*)</a>";
                Regex SplitRegex = new Regex(pattern);
                MatchCollection matches = SplitRegex.Matches(e.Result);
                if (matches.Count > 0)
                {
                    // Getting current folder
                    string CurrentFolder = (Regex.Match(e.Result, "<strong>.*</strong>(.*)</td>")).Groups[1].Value.Replace("\\", "%5C");

                    // Saves the current location
                    ConnectionProfile.BrowsePath = CurrentFolder == "Root" ? "" : CurrentFolder;
                    AppSettings[ConnectionProfile.KeyName] = ConnectionProfile.ToStringArray();

                    foreach (Match match in matches)
                    {
                        BrowseItem bi = new BrowseItem();
                        if (match.Groups[2].Value == "..")
                        {
                            PathBack = match.Groups[1].Value.Replace("\\", "%5C");
                        }
                        else
                        {
                            bi.Name = match.Groups[2].Value;
                            // Replaces backslashs with the urlencoded code (workaround for silverlight bug https://connect.microsoft.com/VisualStudio/feedback/details/649543/)
                            bi.Path = match.Groups[1].Value.Replace("\\", "%5C");
                            BrowseItemsList.Add(bi);
                        }
                    }

                    RefreshListBox();
                    DeactivateProgressBar();
                }
            }
            catch (System.Net.WebException)
            {
                DeactivateProgressBar();
                MessageBox.Show("Connection failed, please check your connection and network settings! Also make sure your remote instance of MPC-HC is running!", "MPC-HC Remote", MessageBoxButton.OK);
                return;
            }
        }
        #endregion

        #region Error TextBlock
        public void ActivateErrorTextBlock()
        {
            if (!this.ContentPanel.Children.Contains(ErrorTextBlock))
                this.ContentPanel.Children.Add(ErrorTextBlock);
        }

        public void DeactivateErrorTextBlock()
        {
            if (this.ContentPanel.Children.Contains(ErrorTextBlock))
                this.ContentPanel.Children.Remove(ErrorTextBlock);
        }


        #endregion

        public void RefreshListBox()
        {
            Dispatcher.BeginInvoke(() =>
                {
                    LB_BrowseItems.ItemsSource = null;
                    LB_BrowseItems.ItemsSource = BrowseItemsList;
                }
            );
        }

        #region De-/Activate Progressbar
        public void ActivateProgressBar()
        {
            if (loadingbar.IsIndeterminate != true)
                loadingbar.IsIndeterminate = true;

            if (!this.ContentPanel.Children.Contains(loadingbar))
                this.ContentPanel.Children.Add(loadingbar);
        }

        public void DeactivateProgressBar()
        {
            if (loadingbar.IsIndeterminate != false)
                loadingbar.IsIndeterminate = false;

            if (this.ContentPanel.Children.Contains(loadingbar))
                this.ContentPanel.Children.Remove(loadingbar);
        }
        #endregion
    }
}