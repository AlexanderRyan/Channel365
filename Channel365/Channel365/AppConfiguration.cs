// ==++==
// 
//   Copyright (c) Teo Jörsin.  All rights reserved.
// 
// ==--==

namespace Channel365 {
    using System;
    using Microsoft.Extensions.Configuration;

    public static class AppConfiguration {
        private static IConfiguration currentConfig;

        public static void SetConfig(IConfiguration configuration) {
            currentConfig = configuration;
        }

        public static string GetConfiguration(string configKey) {
            try {
                string connectionString = currentConfig.GetConnectionString(configKey);
                return connectionString;
            } catch(Exception ex) { throw (ex); }
        }
    }
}