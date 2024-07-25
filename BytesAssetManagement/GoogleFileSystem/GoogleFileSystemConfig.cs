using BytesAssetManagement.FileSystem;

namespace BytesAssetManagement.GoogleFileSystem;

public class GoogleFileSystemConfig: FileSystemConfig
{
    public string ProjectId { get; set; }
    public string BucketName { get; set; }
    public string CredentialsPath { get; set; }
    public string Scope { get; set; } = "https://www.googleapis.com/auth/devstorage.read_only";
}
