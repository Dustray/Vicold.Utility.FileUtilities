using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Vicold.Utility.FileUtilities.FCUtility.Configuration.Entities;

namespace Vicold.Utility.FileUtilities.FCUtility.Configuration
{
    internal class ConfigDriver : IDisposable
    {
        private static readonly string _coreConfigPath = AppDomain.CurrentDomain.BaseDirectory + @"data\\core.json";

        private CustomConfig _customConfig = new CustomConfig();

        public ConfigDriver()
        {
            Init();
        }

        private void Init()
        {
            try
            {
                var json = System.IO.File.ReadAllText(_coreConfigPath);
                var customConfig = JsonSerializer.Deserialize<CustomConfig>(json);
                if(customConfig is{ })
                {
                    _customConfig = customConfig;
                }
            }
            catch
            {
                throw new Exception("Configure file not found.");
            }
        }

        public CustomConfig GetConfig()
        {
            return _customConfig;
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(_customConfig, new JsonSerializerOptions(JsonSerializerDefaults.General));
            System.IO.File.WriteAllText(_coreConfigPath, json);
        }

        public void Dispose()
        {
        }
    }
}
