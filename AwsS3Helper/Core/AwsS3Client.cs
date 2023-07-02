using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3Helper.Core
{
    public class GetObjectModel
    {
        public string ContentType { get; set; }
        public Stream Content { get; set; }
    }
    public class UploadObjectModel
    {
        public bool Success { get; set; }
        public string FileName { get; set; }
    }
    public class AwsS3Client : IAwsS3Client
    {
        private readonly AmazonS3Client _client;

        private readonly AwsS3Options _options;

        private readonly string? _bucketName;

        private readonly IBlobNamingNormalizer _normalizer;

        public AwsS3Client(IOptions<AwsS3Options> options, AmazonS3Client client, IBlobNamingNormalizer normalizer)
        {
            _normalizer = normalizer;
            _options = options.Value;
            _bucketName = options.Value.BucketName;
            if (!string.IsNullOrEmpty(_bucketName))
            {
                _bucketName = _normalizer.NormalizeContainerName(_bucketName);
                options.Value.BucketName = _bucketName;
            }

            _client = client;
            if (!string.IsNullOrEmpty(_bucketName))
            {
                CreateBucketIfNotExistAsync(_bucketName).Wait();
            }
        }

        private string GetBucketName(string? bucketName)
        {
            if (bucketName == null)
            {
                bucketName = _bucketName ?? throw new ArgumentNullException("bucketName");
            }

            return _normalizer.NormalizeContainerName(bucketName);
        }

        public async Task<bool> CreateBucketIfNotExistAsync(string bucketName)
        {
            BucketExistsArgs args = new BucketExistsArgs().WithBucket(bucketName);
            if (!(await _client.BucketExistsAsync(args)))
            {
                MakeBucketArgs args2 = new MakeBucketArgs().WithBucket(bucketName);
                await _client.MakeBucketAsync(args2);
            }

            return true;
        }

        public async Task<Stream> DownloadAsync(string blobName, string folderAddress = "", string? bucketName = null)
        {
            bucketName = GetBucketName(bucketName);
            MemoryStream st = new MemoryStream();
            GetObjectArgs args = new GetObjectArgs().WithBucket(bucketName).WithObject(_normalizer.NormalizeBlobName(blobName, folderAddress)).WithCallbackStream(delegate (Stream stream)
            {
                stream.CopyTo(st);
            });
            await _client.GetObjectAsync(args);
            return st;
        }
        public async Task<string> GetFileAddressAsync(string blobName, string folderAddress = "", int expireAfter = 432000, string? bucketName = null)
        {
            bucketName = GetBucketName(bucketName);
            PresignedGetObjectArgs args = new PresignedGetObjectArgs().WithBucket(bucketName).WithRequestDate(null).WithExpiry(expireAfter)
                .WithObject(string.Join("/", folderAddress, blobName));
            CultureInfo cl = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            string result = await _client.PresignedGetObjectAsync(args).ConfigureAwait(continueOnCapturedContext: false);
            Thread.CurrentThread.CurrentCulture = cl;
            return result;
        }

        public async Task UploadAsync(Stream file, string blobName, string folderAddress = "", string? bucketName = null)
        {
            bucketName = GetBucketName(bucketName);
            PutObjectArgs args = new PutObjectArgs().WithBucket(bucketName).WithObject(_normalizer.NormalizeBlobName(blobName, folderAddress)).WithStreamData(file)
                .WithObjectSize(file.Length)
                .WithContentType(MimeTypesMap.GetMimeType(blobName));
            await _client.PutObjectAsync(args).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task UploadAsync(string fileName, string blobName, string folderAddress = "", string? bucketName = null)
        {
            using FileStream file = new FileStream(fileName, FileMode.Open);
            await UploadAsync(file, blobName, folderAddress, bucketName).ConfigureAwait(continueOnCapturedContext: false);
        }



        public static async Task<GetObjectModel> GetObject(string name)
        {
            var client = new AmazonS3Client(accessKey, accessSecret, Amazon.RegionEndpoint.EUCentral1);

            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucket,
                Key = name
            };
            var responseObject = await client.GetObjectAsync(request);
            return new GetObjectModel
            {

                ContentType = responseObject.Headers.ContentType,
                Content = responseObject.ResponseStream
            };
        }

        public static async Task<UploadObjectModel> UploadObject(IFormFile file)
        {
            // connecting to the client
            var client = new AmazonS3Client(accessKey, accessSecret, Amazon.RegionEndpoint.EUCentral1);

            byte[] fileBytes = new Byte[file.Length];
            file.OpenReadStream().Read(fileBytes, 0, Int32.Parse(file.Length.ToString()));

            var fileName = Guid.NewGuid() + file.FileName;

            PutObjectResponse response = null;

            using (var stream = new MemoryStream(fileBytes))
            {
                var request = new PutObjectRequest
                {
                    BucketName = bucket,
                    Key = fileName,
                    InputStream = stream,
                    ContentType = file.ContentType,
                    CannedACL = S3CannedACL.PublicRead
                };

                response = await client.PutObjectAsync(request);
            };

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                // this model is up to you, in my case I have to use it following;
                return new UploadObjectModel
                {
                    Success = true,
                    FileName = fileName
                };
            }
            else
            {
                return new UploadObjectModel
                {
                    Success = false,
                    FileName = fileName
                };
            }
        }

        public static async Task<UploadObjectModel> RemoveObject(String fileName)
        {
            var client = new AmazonS3Client(accessKey, accessSecret, Amazon.RegionEndpoint.EUCentral1);

            var request = new DeleteObjectRequest
            {
                BucketName = bucket,
                Key = fileName
            };

            var response = await client.DeleteObjectAsync(request);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return new UploadObjectModel
                {
                    Success = true,
                    FileName = fileName
                };
            }
            else
            {
                return new UploadObjectModel
                {
                    Success = false,
                    FileName = fileName
                };
            }
        }
    }

}
