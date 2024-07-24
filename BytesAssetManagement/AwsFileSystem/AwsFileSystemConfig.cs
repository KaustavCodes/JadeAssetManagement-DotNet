using BytesAssetManagement.FileSystem;

namespace BytesAssetManagement.AwsFileSystem;

public class AwsFileSystemConfig: FileSystemConfig
{
    public string BucketName { get; set; }

    public string Region { get; set; }

    public string AccessKey { get; set; }

    public string SecretKey { get; set; }

    public string ServiceUrl { get; set; }

    public string EndpointUrl { get; set; }
}