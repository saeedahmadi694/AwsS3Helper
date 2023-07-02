using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3Helper.Core
{
    public interface IAwsS3Client
    {
        Task CreateNewBucketAsync(string BucketName);

        Task DeleteObject(string BucketName, string fileName);
        Task UploadObjectAsync(string BucketName, string fileName);
        Task UploadFileAsync(string BucketName, string fileName);


        Task UploadAsync(Stream stream, string blobName, string folderAddress = "", string? bucketName = null);
        Task UploadAsync(string fileName, string blobName, string folderAddress = "", string? bucketName = null);
        Task<Stream> DownloadAsync(string blobName, string folderAddress = "", string? bucketName = null);
        Task<string> GetFileAddressAsync(string blobName, string folderAddress = "", int expireAfter = 432000, string? bucketName = null);
    }

}
