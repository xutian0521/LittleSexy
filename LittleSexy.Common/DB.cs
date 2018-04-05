using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace LittleSexy.Common
{
    public static class DB
    {
        public static IDbConnection Conn(string connKey= "ConnStr")
        {
            string connStr = JsonConfigurationHelper.GetAppSettings(connKey);
            return new MySqlConnection(connStr);
        }
    }
    public static class JsonConfigurationHelper
    {
        public static T GetAppSettings<T>(string key) where T : class, new()
        {
            var baseDir = AppContext.BaseDirectory;

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Add(new JsonConfigurationSource { Path = "appsettings.json", Optional = false, ReloadOnChange = true })
                .Build();
            var appconfig = new ServiceCollection()
                .AddOptions()
                .Configure<T>(config.GetSection(key))
                .BuildServiceProvider()
                .GetService<IOptions<T>>()
                .Value;
            return appconfig;
        }
        public static string GetAppSettings(string key)
        {
            var baseDir = AppContext.BaseDirectory;

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Add(new JsonConfigurationSource { Path = "appsettings.json", Optional = false, ReloadOnChange = true })
                .Build();
            
            return config.GetSection(key).Value;
        }
    }

}
