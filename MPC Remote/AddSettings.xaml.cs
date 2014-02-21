using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace MPC_Remote
{
    public partial class AddSettings : PhoneApplicationPage
    {
        private AppSettings AppSettings;

        private ConnectionProfile _ConnectionProfile;

        private bool EditMode = false;

        public AddSettings()
        {
            InitializeComponent();
            AppSettings =  new AppSettings();
            _ConnectionProfile = new ConnectionProfile();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string edit = "";
            string KeyName = "";
            string Description = "";
            string Hostname = "";
            string Port = "";

            if (NavigationContext.QueryString.TryGetValue("edit", out edit))
            {
                PageTitle.Text = "edit settings";
                EditMode = true;
            }
            if (NavigationContext.QueryString.TryGetValue("KeyName", out KeyName))
                _ConnectionProfile.KeyName = KeyName;
            if (NavigationContext.QueryString.TryGetValue("Description", out Description))
                _ConnectionProfile.Description = Description;
            if (NavigationContext.QueryString.TryGetValue("Hostname", out Hostname))
                _ConnectionProfile.Hostname = Hostname;
            if (NavigationContext.QueryString.TryGetValue("Port", out Port))
                _ConnectionProfile.Port = Port;

            this.DataContext = _ConnectionProfile;
        }

        /// <summary>
        /// Open an info MessageBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarHelpButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This app requires a running remote instance of \'Media Player Classic - Home Cinema\' in Version \'1.5.2.3456 (x86 & x64)\' or \'1.6.0.4014 (x86 & x64)\'.\nYou need to enable the build-in \'webinterface\' feature in \'MPC-HC\' to get this work. To do this open \'MPC-HC\' and go to:\n-> View -> Options... -> Web Interface.\nThere, you need to activate \'Listen on port:\' and save the settings.", "MPC-HC Remote", MessageBoxButton.OK);
        }

        /// <summary>
        /// Saves the entered settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarSaveButton_Click(object sender, EventArgs e)
        {
            // Checks if all fields are filled
            if (string.IsNullOrWhiteSpace(textBox_Description.Text) || string.IsNullOrWhiteSpace(textBox_Hostname.Text) || string.IsNullOrWhiteSpace(textBox_Port.Text))
            {
                MessageBox.Show("Not all fields are filled!", "MPC-HC Remote", MessageBoxButton.OK);
                return;
            }

            // Check if hostname contains a ":"
            if (textBox_Hostname.Text.Contains(":"))
            {
                MessageBox.Show("A Hostname has never a colon in it!", "MPC-HC Remote", MessageBoxButton.OK);
                return;
            }

            //Check if port has just numbers in it
            int num;
            bool isNumeric = int.TryParse(textBox_Port.Text, out num);
            if (!isNumeric)
            {
                MessageBox.Show("A port is always numeric!", "MPC-HC Remote", MessageBoxButton.OK);
                return;
            }

            // Check if port isn't bigger than 65535
            if (Convert.ToInt32(textBox_Port.Text) > 65535)
            {
                MessageBox.Show("A port number can never be bigger then 65535!", "MPC-HC Remote", MessageBoxButton.OK);
                return;
            }

            // Actually save some data
            if (EditMode == true)
            {
                AppSettings[_ConnectionProfile.KeyName] = new ConnectionProfile(_ConnectionProfile.KeyName, textBox_Description.Text, textBox_Hostname.Text, textBox_Port.Text, "").ToStringArray();
            }
            else
            {
                ConnectionProfile tempConnectionProfile = new ConnectionProfile(AppSettings.GetNewAppSettingKeyName(), textBox_Description.Text, textBox_Hostname.Text, textBox_Port.Text, "");
                AppSettings[tempConnectionProfile.KeyName] = tempConnectionProfile.ToStringArray();
                AppSettings.DefaultSetting = tempConnectionProfile.KeyName;
            }
            NavigationService.GoBack();
        }

        private void bt_TestConnection_Click(object sender, RoutedEventArgs e)
        {
            // Checks if all fields are filled
            if (string.IsNullOrWhiteSpace(textBox_Hostname.Text) || string.IsNullOrWhiteSpace(textBox_Port.Text))
            {
                MessageBox.Show("Hostname and port fields must been filled, to perform this operation", "MPC-HC Remote", MessageBoxButton.OK);
                return;
            }

            if (CommandClient.IsConnectionAvailable(textBox_Hostname.Text, Convert.ToInt32(textBox_Port.Text)) == true)
            {
                // Checks if all fields are filled AND if yes, promt a MessageBox with save option
                if (string.IsNullOrWhiteSpace(textBox_Description.Text) || string.IsNullOrWhiteSpace(textBox_Hostname.Text) || string.IsNullOrWhiteSpace(textBox_Port.Text))
                {
                    MessageBoxResult msgBoxResult = MessageBox.Show("Connection successful!", "MPC-HC Remote", MessageBoxButton.OKCancel);
                    if (msgBoxResult == MessageBoxResult.OK)
                    {
                        if (EditMode == true)
                        {
                            AppSettings[_ConnectionProfile.KeyName] = _ConnectionProfile.ToStringArray();
                        }
                        else
                        {
                            ConnectionProfile tempConnectionProfile = new ConnectionProfile(AppSettings.GetNewAppSettingKeyName(), textBox_Description.Text, textBox_Hostname.Text, textBox_Port.Text, "");
                            AppSettings[tempConnectionProfile.KeyName] = tempConnectionProfile.ToStringArray();
                            AppSettings.DefaultSetting = tempConnectionProfile.KeyName;
                        }
                        NavigationService.GoBack();
                    }

                }
                else
                {
                    MessageBox.Show("Connection successful!", "MPC-HC Remote", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("Connection failed, please check your connection and network settings! Also make sure your remote instance of MPC-HC is running!", "MPC-HC Remote", MessageBoxButton.OK);
            }

        }
    }
}