using Microsoft.Extensions.Configuration;


namespace WebApplication.DbModels
{
    public class Settings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public IConfigurationRoot iConfigurationRoot;
    }
}