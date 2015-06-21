namespace TfsNotificationRelay.Tests.SlackNotifierSpecification
{
    using System.Configuration;
    using System.Linq;
    using DevCore.TfsNotificationRelay.Configuration;

    internal static class TestConfigurationHelper
    {
        public static BotElement LoadSlackBot(string configFile)
        {
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFile };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            var section = config.GetSection("tfsNotificationRelay") as TfsNotificationRelaySection;

            return section.Bots.FirstOrDefault(bot => bot.Id == "slack");
        }
    }
}