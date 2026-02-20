using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using BusinessLogicLayer.Infrastructure;
using DataAccessLayer.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public enum FileToUpload
    {
        ProfileImage,
        RecipeImage,
        Other
    }
    public class R2Service
    {
        private readonly string? _bucketName;
        private readonly string? _endpoint;
        private readonly string? _publicR2Url;
        private readonly AmazonS3Client _s3Client;
        private readonly ApplicationDbContext _context;

        public R2Service(IConfiguration configuration, ApplicationDbContext context)
        {
            var accessKeyId = configuration["CloudflareR2:AccessKeyId"];
            var secretAccessKey = configuration["CloudflareR2:SecretAccessKey"];
            _bucketName = configuration["CloudflareR2:BucketName"];
            _endpoint = configuration["CloudflareR2:Endpoint"];
            _publicR2Url = configuration["CloudflareR2:PublicR2DevBucketUrl"];

            var config = new AmazonS3Config
            {
                ServiceURL = "https://" + _endpoint,
                ForcePathStyle = true // Required for R2
            };

            _s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, config);
            _context = context;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string sanitizedFileName)
        {
            var uploadRequest = new PutObjectRequest
            {
                InputStream = fileStream,
                Key = sanitizedFileName,
                BucketName = _bucketName,
                ContentType = $"image/{sanitizedFileName.Split('.').Last()}", // Adjust based on your file type,
                DisablePayloadSigning = true
            };

            //await transferUtility.UploadAsync(uploadRequest);
            var response = await _s3Client.PutObjectAsync(uploadRequest);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK && response.HttpStatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new ClientError(400, "Upload to Cloudflare R2 failed");
            }


            // Construct the public URL using the R2.dev subdomain
            var publicUrl = $"{_publicR2Url}/{sanitizedFileName}";
            return publicUrl;

            //var publicUrl = $"https://{_bucketName}.{_endpoint}/{sanitizedFileName}";
            ///return publicUrl;
            //return $"{_endpoint}/{_bucketName}/{fileName}";
        }

        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };

            var response = await _s3Client.GetObjectAsync(request);
            return response.ResponseStream;
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };

            var response = await _s3Client.DeleteObjectAsync(deleteObjectRequest);
            return (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent || response.HttpStatusCode == System.Net.HttpStatusCode.OK);
        }

        public async Task<string> UpdateFileAsync(Stream newFileStream, string oldFileName, string newFileName)
        {
            await DeleteFileAsync(oldFileName);
            return await UploadFileAsync(newFileStream, newFileName);
        }

        public string GetFileUrl(string fileName)
        {
            // Construct the public URL using the R2.dev subdomain
            var publicUrl = $"https://{_bucketName}.{_endpoint}/{fileName}";
            return publicUrl;
        }


        public string SanitizeFileName(string fileName, FileToUpload toUpload)
        {
            // Replace invalid characters with underscores
            var invalidChars = Path.GetInvalidFileNameChars();
            fileName = string.Concat(fileName.Select(c => invalidChars.Contains(c) ? '_' : c));
            if (toUpload == FileToUpload.ProfileImage)
            {
                if (_context.Profiles.Any(x => x.ProfilePictureName == fileName))
                {
                    string uniqueSuffix = $"_{DateTime.UtcNow:yyyyMMddHHmmss}";
                    fileName = Path.GetFileNameWithoutExtension(fileName) + uniqueSuffix + Path.GetExtension(fileName);
                }
                fileName = "profile_images/" + fileName;
            }
            return fileName;
        }

    }
}
