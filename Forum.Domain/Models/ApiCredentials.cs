namespace Forum.Domain.Models
{
    public class ApiCredentials
   {
        public Dictionary<string, object> ConnectionStrings { get; set; }
        public Dictionary<string, object> AWS { get; set; }
        public Dictionary<string, object> Jwt { get; set; }
        public Dictionary<string, object> AmazonSES { get; set; }
        public Dictionary<string, object> MailSettings { get; set; }
        public Dictionary<string, object> Logging { get; set; }
        public Dictionary<string, object> S3 { get; set; }
        public Dictionary<string, object> MongoDbSettings { get; set; }
        public string AllowedHosts { get; set; }
   }
}
