using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3Helper.Core
{
    public interface IAwsS3Client
    {
        //Task CreateNewBucketAsync(string BucketName);

        Task DeleteAsync(string fileName, string folderAddress = "", string? bucketName = null);
        Task UploadAsync(Stream stream, string fileName, string folderAddress = "", string? bucketName = null);
        Task<string> GetFileAddressAsync(string fileName, string folderAddress = "", int expireAfter = 432000, string? bucketName = null);
        Task<Stream> DownloadAsync(string fileName, string folderAddress = "", string? bucketName = null);
    }

}
