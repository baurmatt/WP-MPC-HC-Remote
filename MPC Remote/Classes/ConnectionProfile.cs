using System;

namespace MPC_Remote
{
    public class ConnectionProfile
    {
        /// <summary>
        /// Constructor which initalize an empty instance of ConnectionProfile
        /// </summary>
        public ConnectionProfile()
        {
            this.KeyName = string.Empty;
            this.Description = string.Empty;
            this.Hostname = string.Empty;
            this.Port = string.Empty;
            this.BrowsePath = string.Empty;
        }

        /// <summary>
        /// Constructor which initalize a complete set of Settings
        /// </summary>
        /// <param name="Description">string</param>
        /// <param name="Hostname">string</param>
        /// <param name="Port">string</param>
        /// <param name="BrowsePath">string</param>
        public ConnectionProfile(string KeyName, string Description, string Hostname, string Port, string BrowsePath)
        {
            this.KeyName = KeyName;
            this.Description = Description;
            this.Hostname = Hostname;
            this.Port = Port;
            this.BrowsePath = BrowsePath;
        }

        /// <summary>
        /// Property to get and set the KeyName
        /// </summary>
        public string KeyName { get; set; }

        /// <summary>
        /// Property to get and set the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Property to get and set the hostname
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Property to get and set the port
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// Property to get and set the browse Path
        /// </summary>
        public string BrowsePath { get; set; }

        /// <summary>
        /// Imports an 'AppSettings' String Array with Connection Settings
        /// </summary>
        /// <param name="tempArray"></param>
        public void FromAppSettingsStringArray(string[] tempArray)
        {
            this.Description = tempArray[0];
            this.Hostname = tempArray[1];
            this.Port = tempArray[2];
            this.BrowsePath = tempArray[3];
        }


        /// <summary>
        /// Returns an array convert for use in 'AppSettings'
        /// </summary>
        // string[] goes like this (Description, Hostname, Port, BrowsePath)
        /// <returns>string[]</returns>
        public string[] ToStringArray()
        {
            return new string[] { this.Description, this.Hostname, this.Port, this.BrowsePath };
        }
    }
}
