using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ModeratorBot
{
    /// <summary>
    /// Storage class.
    /// </summary>
    public class Storage
    {
        private string _configFile;
        private string _storageFile;
        
        /// <summary>
        /// New Instance of Storage.
        /// </summary>
        /// <param name="configFile">Path to the config file.</param>
        /// <param name="storageFile">Path to the storage file.</param>
        public Storage(string configFile = "config.json", string storageFile = "storage.json") 
        {
            _configFile = configFile;
            _storageFile = storageFile;
        }

        /// <summary>
        /// Gets BotToken from config file.
        /// </summary>
        /// <returns>Token</returns>
        public string GetToken()
        {
            Dictionary<string, string> config =
            JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(_configFile));
            return config["token"];
        }

        /// <summary>
        /// Gets Prefix from config file.
        /// </summary>
        /// <returns></returns>
        public string GetPrefix()
        {
            Dictionary<string, string> config =
            JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(_configFile));
            return config["prefix"];
        }

        /// <summary>
        /// Gets value by specified key from 'storage' file.
        /// </summary>
        /// <param name="key">What key.</param>
        /// <returns>Value of the specified key.</returns>
        public string GetKeyValue(string key)
        {
            Dictionary<string, string> storage =
            JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(_storageFile));
            return storage[key];
        }
        
    }
}