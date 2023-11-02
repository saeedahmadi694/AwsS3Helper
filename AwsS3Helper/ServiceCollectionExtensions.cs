using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
        public static IServiceCollection AddAwsS3(this IServiceCollection services, Action<AwsS3Options> configure)
        {
            services.Configure(Options.DefaultName, configure);
            services.TryAddSingleton<IBlobNamingNormalizer, AwsS3BlobNamingNormalizer>();
            services.TryAddSingleton<IAwsS3Client, AwsS3Client>();
            return services;
        }
    }
}
