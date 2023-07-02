using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using AwsS3Helper.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
namespace AwsS3Helper
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAwsS3(this IServiceCollection services, string name, AwsS3Options configure)
        {
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.USEast1,
                ServiceURL = configure.Endpoint
            };
            var amazonS3Client = new AmazonS3Client(
              configure.AccessKey,
              configure.SecretKey,
              config);


            services.TryAddSingleton<IBlobNamingNormalizer, AwsS3BlobNamingNormalizer>();
            services.TryAddSingleton<IAwsS3Client, AwsS3Client>();
            return services;
        }
    }
}
