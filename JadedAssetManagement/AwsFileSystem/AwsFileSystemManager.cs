using JadedAssetManagement.Base;
using Microsoft.Extensions.Configuration;

public class AwsFileSystemManager : IFileSystemBase
{
    private int _pageSize = 10;

    private string _rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

    /// <summary>
    /// This constructor accepts a dynamic object and expects a parameter called PageSize.
    /// </summary>
    /// <param name="configuration">PageSize as integer. eg 10</param>
    public AwsFileSystemManager(FileSystemConfig configuration)
    {
        if (configuration.PageSize != null)
            _pageSize = configuration.PageSize;
        
        if (configuration.RootPath != null)
            _rootPath = configuration.RootPath;
    }

    public AwsFileSystemManager(IConfiguration configuration)
    {
        _pageSize = Convert.ToInt32(configuration["JadedFileSystemConfig:PageSize"]);
        _rootPath = configuration["JadedFileSystemConfig:RootPath"].ToString();
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