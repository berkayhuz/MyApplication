using Microsoft.Extensions.Configuration;

namespace Forum.Infrastructure.Configuration
{
    public static class ConfigurationExtensions
    {
        public static void AddAmazonSecretsManager(this IConfigurationBuilder configurationBuilder,
                                                    string region,
                                                    string secretName)
        {
            var configurationSource = new AmazonSecretsManagerConfigurationSource(region, secretName);
            configurationBuilder.Add(configurationSource);
        }
    }

}
