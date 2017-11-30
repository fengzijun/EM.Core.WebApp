using System;
using System.Configuration;
using System.Xml;

namespace EM.Configuration
{
    /// <summary>
    /// ≈‰÷√–≈œ¢
    /// </summary>
    public class SystemConfig 
    {
        /// <summary>
        /// Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {


            var config = new SystemConfig();

            //var redisCachingNode = section.SelectSingleNode("RedisCaching");
            //if (redisCachingNode != null && redisCachingNode.Attributes != null)
            //{
            //    var connectionStringAttribute = redisCachingNode.Attributes["ConnectionString"];
            //    if (connectionStringAttribute != null)
            //        config.RedisCachingConnectionString = connectionStringAttribute.Value;

            //    var portAttribute = redisCachingNode.Attributes["port"];
            //    if (portAttribute != null)
            //        config.RedisPort = portAttribute.Value;
            //}

            //var mongosettingnode = section.SelectSingleNode("MongoSetting");
            //if (mongosettingnode != null && mongosettingnode.Attributes != null)
            //{
            //    var connectionStringAttribute = mongosettingnode.Attributes["ConnectionString"];
            //    if (connectionStringAttribute != null)
            //        config.MongoConnectionString = connectionStringAttribute.Value;
            //}

            //var mssqlsettingnode = section.SelectSingleNode("MsSqlSetting");
            //if (mssqlsettingnode != null && mssqlsettingnode.Attributes != null)
            //{
            //    var connectionStringAttribute = mssqlsettingnode.Attributes["ConnectionString"];
            //    if (connectionStringAttribute != null)
            //        config.MsSqlConnectionString = connectionStringAttribute.Value;
            //}

            //var mswritesqlsettingnode = section.SelectSingleNode("MsWriteSqlSetting");
            //if (mswritesqlsettingnode != null && mswritesqlsettingnode.Attributes != null)
            //{
            //    var connectionStringAttribute = mswritesqlsettingnode.Attributes["ConnectionString"];
            //    if (connectionStringAttribute != null)
            //        config.MsSqlWriteConnectionString = connectionStringAttribute.Value;
            //}

            //var startupNode = section.SelectSingleNode("Startup");
            //if (startupNode != null && startupNode.Attributes != null)
            //{
            //    var attribute = startupNode.Attributes["IgnoreStartupTasks"];
            //    if (attribute != null)
            //        config.IgnoreStartupTasks = Convert.ToBoolean(attribute.Value);
            //}

            //var dataProvidernode = section.SelectSingleNode("DataProvider");
            //if (dataProvidernode != null && dataProvidernode.Attributes != null)
            //{
            //    var attribute = dataProvidernode.Attributes["Name"];
            //    if (attribute != null)
            //        config.DataProviderName = attribute.Value;
            //}

            //var databaseinstallnode = section.SelectSingleNode("DatabaseInstall");
            //if (databaseinstallnode != null && databaseinstallnode.Attributes != null)
            //{
            //    var attribute = databaseinstallnode.Attributes["DatabaseInstallModel"];
            //    if (attribute != null)
            //        config.DatabaseInstallModel = Convert.ToInt32(attribute.Value);

            //    attribute = databaseinstallnode.Attributes["DatabaseIsInstalled"];
            //    if (attribute != null)
            //        config.DatabaseIsInstalled = Convert.ToBoolean(attribute.Value);
            //}

            //var SystemSettingnode = section.SelectSingleNode("SystemSetting");
            //if (SystemSettingnode != null && SystemSettingnode.Attributes != null)
            //{
            //    config.SystemSetting = new SystemSetting();
            //    var attribute = SystemSettingnode.Attributes["Version"];
            //    if (attribute != null)
            //        config.SystemSetting.Version = attribute.Value;

            //    attribute = SystemSettingnode.Attributes["IsWebApi"];
            //    if (attribute != null)
            //        config.SystemSetting.IsWebApi = Convert.ToBoolean(attribute.Value);

            //    attribute = SystemSettingnode.Attributes["IsDev"];
            //    if (attribute != null)
            //        config.SystemSetting.IsDev = Convert.ToBoolean(attribute.Value);
            //}

            return config;
        }

        public string RedisCachingConnectionString { get; private set; }

        public string RedisPort { get; private set; }

        public string MongoConnectionString { get; private set; }

        public string MsSqlConnectionString { get; private set; }

        public string MsSqlWriteConnectionString { get; private set; }

        public bool IgnoreStartupTasks { get; private set; }

        public string DataProviderName { get; private set; }

        public int DatabaseInstallModel { get; private set; }

        public bool DatabaseIsInstalled { get; set; }

        public string Version { get; set; }

        public bool IsWebApi { get; set; }

        public bool IsDev { get; set; }
    }

}