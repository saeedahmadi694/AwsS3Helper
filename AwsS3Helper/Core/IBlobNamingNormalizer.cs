using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3Helper.Core
{
    public interface IBlobNamingNormalizer
    {
        string NormalizeContainerName(string containerName);

        string NormalizeBlobName(string blobName, string folderStruct = "");
    }
}
