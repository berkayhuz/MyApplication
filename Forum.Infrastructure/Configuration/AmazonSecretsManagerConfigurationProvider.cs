using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Forum.Domain.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Text.Json;

namespace Forum.Infrastructure.Configuration
{
    public class AmazonSecretsManagerConfigurationProvider : ConfigurationProvider
    {
        private readonly string _region;
        private readonly string _secretName;
        private readonly ILogger _logger;

        public AmazonSecretsManagerConfigurationProvider(string region, string secretName)
        {
            _region = region;
            _secretName = secretName;
            _logger = Log.ForContext("Amazon", "AmazonSecretsManager");
        }

        public override void Load()
        {
            _logger.Information("Starting Load operation.");
            var secret = GetSecret();

            try
            {
                if (string.IsNullOrEmpty(secret))
                {
                    throw new Exception("Secret is null or empty.");
                }

                ApiCredentials apiCredentials = null;
                try
                {
                    apiCredentials = JsonSerializer.Deserialize<ApiCredentials>(secret);
                    if (apiCredentials == null)
                    {
                        throw new Exception("Data from SecretsManager is not valid.");
                    }
                }
                catch (JsonException jsonEx)
                {
                    _logger.Error(jsonEx, "JSON Deserialization failed.");
                    throw new Exception("JSON Deserialization failed", jsonEx);
                }

                var memoryCollection = new Dictionary<string, string>();

                // AllowedHosts
                if (apiCredentials.AllowedHosts != null)
                {
                    memoryCollection.Add("AllowedHosts", apiCredentials.AllowedHosts);
                }
                else
                {
                    throw new Exception("AllowedHosts is null in API credentials.");
                }

                // JWT Configurations
                if (apiCredentials.Jwt != null && apiCredentials.Jwt.ContainsKey("SecretKey") && apiCredentials.Jwt.ContainsKey("Issuer") && apiCredentials.Jwt.ContainsKey("Audience"))
                {
                    memoryCollection.Add("Jwt:SecretKey", apiCredentials.Jwt["SecretKey"].ToString());
                    memoryCollection.Add("Jwt:Audience", apiCredentials.Jwt["Audience"].ToString());
                    memoryCollection.Add("Jwt:Issuer", apiCredentials.Jwt["Issuer"].ToString());
                }
                else
                {
                    throw new Exception("Jwt is missing required values in API credentials.");
                }

                // S3 Bucket Configuration
                if (apiCredentials.S3 != null && apiCredentials.S3.ContainsKey("BucketName"))
                {
                    memoryCollection.Add("S3:BucketName", apiCredentials.S3["BucketName"].ToString());
                }
                else
                {
                    throw new Exception("S3 configuration is missing in API credentials.");
                }

                // Identity & Profile DB Connection Strings
                if (apiCredentials.ConnectionStrings != null &&
                    apiCredentials.ConnectionStrings.ContainsKey("IdentityDbConnection") &&
                    apiCredentials.ConnectionStrings.ContainsKey("ProfileDbConnection"))
                {
                    memoryCollection.Add("ConnectionStrings:IdentityDbConnection", apiCredentials.ConnectionStrings["IdentityDbConnection"].ToString());
                    memoryCollection.Add("ConnectionStrings:ProfileDbConnection", apiCredentials.ConnectionStrings["ProfileDbConnection"].ToString());
                }
                else
                {
                    throw new Exception("Database connection strings are missing in API credentials.");
                }

                // MongoDB Configuration
                if (apiCredentials.MongoDbSettings != null &&
                    apiCredentials.MongoDbSettings.ContainsKey("ConnectionString") &&
                    apiCredentials.MongoDbSettings.ContainsKey("DatabaseName") &&
                    apiCredentials.MongoDbSettings.ContainsKey("CollectionName"))
                {
                    memoryCollection.Add("MongoDbSettings:ConnectionString", apiCredentials.MongoDbSettings["ConnectionString"].ToString());
                    memoryCollection.Add("MongoDbSettings:DatabaseName", apiCredentials.MongoDbSettings["DatabaseName"].ToString());
                    memoryCollection.Add("MongoDbSettings:CollectionName", apiCredentials.MongoDbSettings["CollectionName"].ToString());
                }
                else
                {
                    throw new Exception("MongoDB settings are missing in API credentials.");
                }

                // MailSettings
                if (apiCredentials.MailSettings != null)
                {
                    foreach (var kvp in apiCredentials.MailSettings)
                    {
                        if (kvp.Key != null && kvp.Value != null)
                        {
                            memoryCollection.Add($"MailSettings:{kvp.Key}", kvp.Value.ToString());
                        }
                    }
                }
                else
                {
                    throw new Exception("MailSettings is missing in API credentials.");
                }

                var builder = new ConfigurationBuilder();
                try
                {
                    builder.AddInMemoryCollection(memoryCollection);

                    foreach (var kvp in memoryCollection)
                    {
                        _logger.Information("MemoryCollection Key: {Key}, Value: {Value}", kvp.Key, kvp.Value);
                    }

                    var config = builder.Build();
                    this.Data = config.AsEnumerable().ToDictionary(k => k.Key, v => v.Value);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Configuration building failed.");
                    throw new Exception("Configuration building failed", ex);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred during Load operation.");
                throw new Exception("Error occurred during Load operation.", ex);
            }
        }

        private string GetSecret()
        {
            _logger.Information("Retrieving secret from SecretsManager.");
            var request = new GetSecretValueRequest
            {
                SecretId = _secretName,
                VersionStage = "AWSCURRENT"
            };

            using (var client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(_region)))
            {
                var response = client.GetSecretValueAsync(request).Result;

                string secretString = string.Empty;

                if (response.SecretString != null)
                {
                    secretString = response.SecretString;
                }
                else
                {
                    var memoryStream = response.SecretBinary;
                    var reader = new StreamReader(memoryStream);
                    secretString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
                }

                return secretString;
            }
        }
    }
}
