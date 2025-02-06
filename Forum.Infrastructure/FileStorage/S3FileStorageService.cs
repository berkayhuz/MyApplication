using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Forum.Infrastructure.FileStorage
{
    public interface IS3FileStorageService
    {
        Task<string> UploadImageAsync(IFormFile imageFile);
    }
    public class S3FileStorageService : IS3FileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        public S3FileStorageService(IConfiguration configuration, IAmazonS3 s3Client = null)
        {
            _s3Client = s3Client ?? configuration.GetAWSOptions().CreateServiceClient<IAmazonS3>();
            _bucketName = configuration["S3:BucketName"];
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageFile.ContentType))
                {
                    throw new ArgumentException("ContentType cannot be empty or null.");
                }
                if (string.IsNullOrWhiteSpace(imageFile.FileName))
                {
                    throw new ArgumentException("Filename cannot be empty or null.");
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileName,
                    InputStream = imageFile.OpenReadStream(),
                    ContentType = imageFile.ContentType
                };

                var response = await _s3Client.PutObjectAsync(putRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
                }

                throw new Exception("Image upload failed.");
            }
            catch (AmazonS3Exception)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Error uploading image", ex);
            }
        }
    }
}
