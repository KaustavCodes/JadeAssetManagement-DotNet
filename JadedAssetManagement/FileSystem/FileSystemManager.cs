using JadedAssetManagement.Base;
using Microsoft.Extensions.Configuration;

namespace JadedAssetManagement.FileSystem;

public class FileSystemManager : IFileSystemBase
{
    private int _pageSize = 10;
    private string _fielpath = String.Empty;
    public int PageSize { 
        get {
            return _pageSize;
        } 
        set {
            if (value <= 0)
            {
                throw new ArgumentException("Page size must be greater than 0.");
            }
            _pageSize = value;
        }
    }

    public Task DeleteDirectoryAsync(string filePath)
    {
        throw new NotImplementedException();
    }

    public Task DeleteFileAsync(string filePath)
    {
        throw new NotImplementedException();
    }

    public FileSystemManager(dynamic configuration)
    {
        throw new NotImplementedException();
    }

    public FileSystemManager(IConfiguration configuration)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AssetTypes>> ListFilesAllFiles()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AssetTypes>> ListFilesAsync(int currentPage)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AssetTypes>> ListFilesAsync(int currentPage, int pgSize, string searchKey = "")
    {
        throw new NotImplementedException();
    }

    public Task UploadFileAsync(byte[] fileContent, string filePath, string destination)
    {
        throw new NotImplementedException();
    }

    public Task<AssetTypes> GetFileAsync(string filePath)
    {
        throw new NotImplementedException();
    }
}
