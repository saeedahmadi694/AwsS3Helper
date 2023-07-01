using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AwsS3Helper.Core
{
    public class AwsS3BlobNamingNormalizer : IBlobNamingNormalizer
    {
        //
        // Summary:
        //     https://docs.aws.amazon.com/AmazonS3/latest/dev/BucketRestrictions.html
        public virtual string NormalizeContainerName(string containerName)
        {
            containerName = containerName.ToLower();
            if (containerName.Length > 63)
            {
                containerName = containerName.Substring(0, 63);
            }

            containerName = Regex.Replace(containerName, "[^a-z0-9-.]", string.Empty);
            containerName = Regex.Replace(containerName, "\\.{2,}", ".");
            containerName = Regex.Replace(containerName, "-\\.", string.Empty);
            containerName = Regex.Replace(containerName, "\\.-", string.Empty);
            containerName = Regex.Replace(containerName, "^-", string.Empty);
            containerName = Regex.Replace(containerName, "-$", string.Empty);
            containerName = Regex.Replace(containerName, "^\\.", string.Empty);
            containerName = Regex.Replace(containerName, "\\.$", string.Empty);
            containerName = Regex.Replace(containerName, "^(?:(?:^|\\.)(?:2(?:5[0-5]|[0-4]\\d)|1?\\d?\\d)){4}$", string.Empty);
            if (containerName.Length < 3)
            {
                int length = containerName.Length;
                for (int i = 0; i < 3 - length; i++)
                {
                    containerName += "0";
                }
            }

            return containerName;
        }

        public virtual string NormalizeBlobName(string blobName, string folderStruct = "")
        {
            if (!string.IsNullOrEmpty(folderStruct))
            {
                folderStruct = folderStruct.Replace("\\", "/") ?? "";
            }

            string text = string.Join("/", folderStruct, blobName);
            if (text.StartsWith("/"))
            {
                text = text.Substring(1);
            }

            return text;
        }
    }

}
