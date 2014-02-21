using System;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.ComponentModel;

namespace MPC_Remote
{
    public class AppSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged; 

        static IsolatedStorageSettings isolatedStore;

        // Isolated Storage Key Names
        const string FirstTimeStartedKeyName = "FirstTimeStarted";
        const string SelectedSettingsKeyName = "SelectedSettings";

        // Default value of Settings
        const bool FirstTimeStartedDefault = false;
        const string SelectedSettingsDefault = "";

        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        public AppSettings()
        {
            try
            {
                isolatedStore = IsolatedStorageSettings.ApplicationSettings;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception while using IsolatedStorageSettings: " + e.ToString());
            }
        }

        /// <summary>
        /// Property to get and set the Settings
        /// </summary>
        public string[] this[string KeyName]
        {
            get
            {
                if (isolatedStore.Contains(KeyName))
                {
                    return (string[])isolatedStore[KeyName];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (isolatedStore.Contains(KeyName))
                {
                    if (isolatedStore[KeyName] != value)
                    {
                        isolatedStore[KeyName] = value;
                    }
                }
                else
                {
                    isolatedStore.Add(KeyName, value);
                }
            }
        }

        /// <summary>
        /// Removes an Setting Pair
        /// </summary>
        /// <param name="KeyName"></param>
        public void RemoveSetting(string KeyName)
        {
            isolatedStore.Remove(KeyName);
        }

        /// <summary>
        /// Gets a collection that contains the keys in the dictionary.
        /// </summary>
        public System.Collections.ICollection Keys
        {
            get
            {
                return isolatedStore.Keys;
            }
        }

        /// <summary>
        /// Gets a collection that contains the values in the dictionary.
        /// </summary>
        public System.Collections.ICollection Values
        {
            get
            {
                return isolatedStore.Values;
            }
        }

        /// <summary>
        /// Gets the number of key-value pairs that are stored in the dictionary.
        /// </summary>
        public int Count
        {
            get
            {
                return isolatedStore.Count;
            }
        }

        /// <summary>
        /// Gets a new and unique KeyName for usage with 'AppSetting' Class
        /// </summary>
        /// <returns></returns>
        public static string GetNewAppSettingKeyName()
        {
            string PossibleNewKeyName = "";
            Boolean Found = false;

            for (int i = 0; Found != true; i++)
            {
                PossibleNewKeyName = "appsettings" + i.ToString();
                Found = true;
                foreach (string setting in isolatedStore.Keys)
                {
                    if (setting == PossibleNewKeyName)
                        Found = false;
                }
            }

            return PossibleNewKeyName;
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (isolatedStore.Contains(Key))
            {
                // If the value has changed
                if (isolatedStore[Key] != value)
                {
                    // Store the new value
                    isolatedStore[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                isolatedStore.Add(Key, value);
                valueChanged = true;
            }

            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public valueType GetValueOrDefault<valueType>(string Key, valueType defaultValue)
        {
            valueType value;

            // If the key exists, retrieve the value.
            if (isolatedStore.Contains(Key))
            {
                value = (valueType)isolatedStore[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }

            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            isolatedStore.Save();
        }

        /// <summary>
        /// Property to get and set if program is started for the first time.
        /// </summary>
        public bool FirstTimeStarted
        {
            get
            {
                return GetValueOrDefault<bool>(FirstTimeStartedKeyName, FirstTimeStartedDefault);
            }
            set
            {
                AddOrUpdateValue(FirstTimeStartedKeyName, value);
                Save();
            }
        }

        /// <summary>
        /// Property to get and set the default Setting name
        /// </summary>
        public string DefaultSetting
        {
            get
            {
                return GetValueOrDefault<string>(SelectedSettingsKeyName, SelectedSettingsDefault);
            }
            set
            {
                AddOrUpdateValue(SelectedSettingsKeyName, value);
                OnPropertyChanged("DefaultSetting");
                Save();
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }
}
