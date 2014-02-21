using System;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Reflection;
using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;

namespace MPC_Remote
{
    public partial class Info : PhoneApplicationPage
    {
        public Info()
        {
            InitializeComponent();
            tb_AppVersion.Text = "Version: " + AssemblyExtensions.GetFileVersion(Assembly.GetExecutingAssembly()).ToString();
        }

        private void bt_SendFeedback_Click(object sender, RoutedEventArgs e)
        {
            // Get device info
            string Manufacturer = DeviceStatus.DeviceManufacturer;
            string Modell = DeviceStatus.DeviceName;
            string OSVersion = Environment.OSVersion.Version.ToString();
            string FirmwareVersion = DeviceStatus.DeviceFirmwareVersion;
            string HardwareVersion = DeviceStatus.DeviceHardwareVersion;
            string ApplicationMemoryUsage = (DeviceStatus.ApplicationCurrentMemoryUsage / (1024 * 1024)).ToString();
            string PeakMemoryUsage = (DeviceStatus.ApplicationPeakMemoryUsage / (1024 * 1024)).ToString();
            string TotalMemory = (DeviceStatus.DeviceTotalMemory / (1024 * 1024)).ToString();

            EmailComposeTask SendFeedbackMail = new EmailComposeTask();
            SendFeedbackMail.To = "mpc-hc-remote@live.de";
            SendFeedbackMail.Subject = "MPC-HC Remote - Feedback";
            SendFeedbackMail.Body = "\n\n\n---------\n" +
            "Manufacturer: " + Manufacturer + "\n" +
            "Model: " + Modell + "\n" +
            "Operating System: Windows Phone " + OSVersion.ToString() + "\n" +
            "Firmware Version: " + FirmwareVersion + "\n" +
            "Hardware Version: " + HardwareVersion + "\n" +
            "Memory Usage: " + ApplicationMemoryUsage + "\n" +
            "Peak Memory Usage: " + PeakMemoryUsage + "\n" +
            "Total Memory: " + TotalMemory;

            SendFeedbackMail.Show();
        }

        private void bt_Review_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

    }

    public static class AssemblyExtensions
    {
        public static Version GetFileVersion(this Assembly assembly)
        {
            var versionString = assembly.GetCustomAttributes(false)
                .OfType<AssemblyFileVersionAttribute>()
                .First()
                .Version;

            return Version.Parse(versionString);
        }
    }
}