namespace RostamBot.Application.Settings
{
    public class RostamBotSettings
    {
        public string DatabaseConnectionString { get; set; }

        public string JwtKey { get; set; }

        public string JwtIssuer { get; set; }

        public string ManagerTwitterAppConsumerKey { get; set; }

        public string ManagerTwitterAppConsumerSecret { get; set; }

        public string ManagerTwitterAppUserAccessToken { get; set; }

        public string ManagerTwitterAppUserAccessSecret { get; set; }

        public string TwitterAppConsumerKey { get; set; }

        public string TwitterAppConsumerSecret { get; set; }

        public string DefaultAdminEmail { get; set; }

        public string DefaultAdminRole { get; set; }

        public string DefaultAdminPassword { get; set; }

        public string TwitterProxy { get; set; }

        public string JobDashboardUsername { get; set; }

        public string JobDashboardPassword { get; set; }

    }
}
