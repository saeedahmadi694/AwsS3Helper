using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace AwsS3Helper.Core
{
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
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.USEast1,
                ServiceURL = options.Value.Endpoint
            };
            _client = new AmazonS3Client(options.Value.AccessKey, options.Value.SecretKey, config);
        }

        private string GetBucketName(string? bucketName)
        {
            if (bucketName == null)
            {
                bucketName = _bucketName ?? throw new ArgumentNullException("bucketName");
            }

            return _normalizer.NormalizeContainerName(bucketName);
        }


        //public async Task<Stream> DownloadAsync(string fileName, string folderAddress = "", string? bucketName = null)
        //{
        //    bucketName = GetBucketName(bucketName);
        //    MemoryStream st = new MemoryStream();
        //    GetObjectArgs args = new GetObjectArgs().WithBucket(bucketName).WithObject(_normalizer.NormalizeBlobName(blobName, folderAddress)).WithCallbackStream(delegate (Stream stream)
        //    {
        //        stream.CopyTo(st);
        //    });
        //    await _client.GetObjectAsync(args);
        //    return st;
        //}
        public async Task<string> GetFileAddressAsync(string fileName, string folderAddress = "", int expireAfter = 432000, string? bucketName = null)
        {
            bucketName = GetBucketName(bucketName);


            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = $"/{folderAddress}/{fileName}"
            };
            var responseObject = await _client.GetObjectAsync(request);
            //if (responseObject)
            //{
            //    return responseObject.
            //}
            return "";
        }

        public async Task DeleteAsync(string fileName, string folderAddress = "", string? bucketName = null)
        {
            bucketName = GetBucketName(bucketName);

            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = fileName,
            };

            var response = await _client.DeleteObjectAsync(request);
        }

        public async Task UploadAsync(Stream stream, string fileName, string folderAddress = "", string? bucketName = null)
        {
            bucketName = GetBucketName(bucketName);


            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fileName,
                InputStream = stream,
                CannedACL = S3CannedACL.PublicRead
            };

            var response = await _client.PutObjectAsync(request);
        }

        public Task<Stream> DownloadAsync(string fileName, string folderAddress = "", string? bucketName = null)
        {
            throw new NotImplementedException();
        }
    }

}
