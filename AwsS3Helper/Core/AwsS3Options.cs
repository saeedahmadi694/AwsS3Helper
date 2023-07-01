using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3Helper.Core
{
    public class AwsS3Options
    {
        public string Endpoint { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string SessionToken { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
    }

}
