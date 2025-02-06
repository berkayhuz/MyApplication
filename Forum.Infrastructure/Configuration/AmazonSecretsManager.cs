using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace Forum.Infrastructure.Configuration
{
    public class AmazonSecretsManager
    {
        private readonly IAmazonSecretsManager _secretsManager;
        private readonly string _secretName;

        public AmazonSecretsManager(IAmazonSecretsManager secretsManager, string secretName)
        {
            _secretsManager = secretsManager;
            _secretName = secretName;
        }

        public async Task<string> GetSecretValueAsync()
        {
            try
            {
                var request = new GetSecretValueRequest
                {
                    SecretId = _secretName
                };

                var response = await _secretsManager.GetSecretValueAsync(request);
                return response.SecretString;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving secret from Secrets Manager.", ex);
            }
        }
    }
}
