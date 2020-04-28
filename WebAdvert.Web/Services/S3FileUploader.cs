using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace WebAdvert.Web.Services
{
    public class S3FileUploader : IFileUploader
    {
        private readonly IConfiguration _configuration;

        public S3FileUploader(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> UploadFileAsync(string fileName, Stream storageStream)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("File name must be specified.");

            string bucketName = _configuration.GetValue<string>("ImageBucket");

            using (AmazonS3Client client = new AmazonS3Client())
            {
                if (storageStream.Length > 0)
                    if (storageStream.CanSeek)
                        storageStream.Seek(0, SeekOrigin.Begin);

                PutObjectRequest request = new PutObjectRequest
                {
                    AutoCloseStream = true,
                    BucketName = bucketName,
                    InputStream = storageStream,
                    Key = fileName
                };

                PutObjectResponse response = await client.PutObjectAsync(request);

                return response.HttpStatusCode == HttpStatusCode.OK;
            }
        }
    }
}