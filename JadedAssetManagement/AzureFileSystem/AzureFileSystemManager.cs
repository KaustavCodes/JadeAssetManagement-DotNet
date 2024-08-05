
using JadedAssetManagement.Base;
using Microsoft.Extensions.Configuration;

namespace JadedAssetManagement.AzureFileSystem;

public class AzureFileSystemManager: IFileSystemBase
{
    public AzureFileSystemManager(IConfiguration configuration)
    {
        string connectionString = configuration["AzureFileSystem:ConnectionString"];
        if(string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException("ConnectionString is missing in the configuration");
        }
    }

    public Task<bool> DeleteDirectoryAsync(string relativeDirectoryPath)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteFileAsync(string relativeFilePath)
    {
        throw new NotImplementedException();
    }

    public Task<AssetTypes> GetFileAsync(string relativeFilePath)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AssetTypes>> ListFilesAllFiles(string relativePath = "/", string searchKey = "")
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AssetTypes>> ListFilesPaged(string relativePath, int currentPage, string searchKey = "", int pageSize = 0)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UploadFileAsync(byte[] fileContent, string relativeDestinationPath)
    {
        throw new NotImplementedException();
    }
}