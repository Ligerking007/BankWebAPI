using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Core.Common
{
    public static class ConfigManage
    {
        public static IConfiguration AppSetting { get; }
        static ConfigManage()
        {
            string path = Directory.GetCurrentDirectory();
            AppSetting = new ConfigurationBuilder()
                    .SetBasePath(path)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
        }
    }
}