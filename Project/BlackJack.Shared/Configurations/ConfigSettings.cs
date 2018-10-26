using BlackJack.Shared.Constants;
using BlackJack.Shared.Enums;

namespace BlackJack.Shared.Configurations
{
    public class ConfigSettings
    {
        public static string ConnectionStringName { get; private set; }

        public static string ConnectionString { get; private set; }

        private static BuildConfiguration _buildConfiguration = BuildConfiguration.Release;

        static ConfigSettings()
        {
            #if DEBUG
            _buildConfiguration = BuildConfiguration.Debug;
            #endif

            if (_buildConfiguration == BuildConfiguration.Debug)
            {
                ConnectionStringName = DbConstants.ConnectionStringToLocalhost;
            }

            if (_buildConfiguration == BuildConfiguration.Release)
            {
                ConnectionStringName = DbConstants.ConnectionStringToLiveServer;
            }
        }
    }
}