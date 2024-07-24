using BytesAssetManagement.FileSystem;

namespace BytesAssetManagement.FileSystem;

public class FileSystemConfig
{
    public int PageSize { get; set; } = 10;
    public string RootPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
}