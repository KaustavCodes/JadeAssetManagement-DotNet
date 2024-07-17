using JadedAssetManagement.Base;
using Microsoft.Extensions.Configuration;

namespace JadedAssetManagement.FileSystem;

public class FileSystemManager : IFileSystemBase
{
    private int _pageSize = 10;

    /// <summary>
    /// This constructor accepts a dynamic object and expects a parameter called PageSize.
    /// </summary>
    /// <param name="configuration">PageSize as integer. eg 10</param>
    public FileSystemManager(dynamic configuration)
    {
        if (configuration.PageSize != null)
            _pageSize = configuration.PageSize;
    }

    public FileSystemManager(IConfiguration configuration)
    {
        _pageSize = Convert.ToInt32(configuration["ByteFiles:PageSize"]);
    }

    public async Task DeleteDirectoryAsync(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            Directory.Delete(directoryPath, true);
            await Task.CompletedTask;
        }
        else
        {
            throw new DirectoryNotFoundException($"The directory '{directoryPath}' was not found.");
        }
    }

    public async Task DeleteFileAsync(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            await Task.CompletedTask;
        }
        else
        {
            throw new FileNotFoundException($"The file '{filePath}' was not found.");
        }
    }

    public async Task<IEnumerable<AssetTypes>> ListFilesAllFiles(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"The directory '{path}' was not found.");
        }

        var assetList = new List<AssetTypes>();

        // Add directories to the list
        foreach (var directoryPath in Directory.GetDirectories(path))
        {
        DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
            assetList.Add(new AssetTypes
            {
                Name = dirInfo.Name,
                IsFolder = true,
                Path = dirInfo.FullName,
                Extension = "",
                MimeType = "directory/folder",
                SizeInBytes = 0, // Directories themselves don't have a size
                DateCreated = dirInfo.CreationTime,
                DateModified = dirInfo.LastWriteTime
            });
        }

        // Add files to the list
        foreach (var filePath in Directory.GetFiles(path))
        {
            FileInfo fileInfo = new FileInfo(filePath);
            assetList.Add(new AssetTypes
            {
                Name = fileInfo.Name,
                IsFolder = false,
                Path = fileInfo.FullName,
                Extension = fileInfo.Extension,
                MimeType = Helpers.GetMimeType(fileInfo.Extension),
                SizeInBytes = (int)fileInfo.Length, // This might need adjustment for large files
                DateCreated = fileInfo.CreationTime,
                DateModified = fileInfo.LastWriteTime
            });
        }

        return await Task.FromResult(assetList);
    }

    public Task<IEnumerable<AssetTypes>> ListFilesAsync(string path, int currentPage)
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
