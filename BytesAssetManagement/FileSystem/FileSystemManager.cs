using BytesAssetManagement.Base;
using Microsoft.Extensions.Configuration;

namespace BytesAssetManagement.FileSystem;

public class FileSystemManager : IFileSystemBase
{
    private int _pageSize = 10;

    private string _rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

    /// <summary>
    /// This constructor accepts a dynamic object and expects a parameter called PageSize.
    /// </summary>
    /// <param name="configuration">PageSize as integer. eg 10</param>
    public FileSystemManager(FileSystemConfig configuration)
    {
        if (configuration.PageSize != null)
            _pageSize = configuration.PageSize;
        
        if (configuration.RootPath != null)
            _rootPath = configuration.RootPath;
    }

    public FileSystemManager(IConfiguration configuration)
    {
        _pageSize = Convert.ToInt32(configuration["JadedFileSystemConfig:PageSize"]);
        _rootPath = configuration["JadedFileSystemConfig:RootPath"].ToString();
    }

    public async Task<bool> DeleteDirectoryAsync(string relativeDirectoryPath)
    {
        var directoryPath = Path.Combine(_rootPath, relativeDirectoryPath);

        try
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
                await Task.CompletedTask;
                return true;
            }
            else
            {
                return false;
            }
        }
        catch(Exception ex)
        {
            throw new DirectoryNotFoundException($"The directory '{directoryPath}' was not found.");
        }
    }

    public async Task<bool> DeleteFileAsync(string relateiveFilePath)
    {
        var filePath = Path.Combine(_rootPath, relateiveFilePath);
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                await Task.CompletedTask;
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            throw new FileNotFoundException($"The directory '{filePath}' was not found.");
        }
    }

    public async Task<IEnumerable<AssetTypes>> ListFilesAllFiles(string relativePath = "", string searchKey = "")
    {
        var path = Path.Combine(_rootPath, relativePath);
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"The directory '{path}' was not found.");
        }

        var assetList = new List<AssetTypes>();
        var searchPattern = string.IsNullOrEmpty(searchKey) ? "*" : $"*{searchKey}*";

        // Add directories to the list
        foreach (var directoryPath in Directory.GetDirectories(path, searchPattern))
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


        // Add files to the list sorting it by name ascending
        foreach (var filePath in Directory.GetFiles(path, searchPattern).OrderBy(f => f))
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

    public async Task<IEnumerable<AssetTypes>> ListFilesPaged(string relativePath, int currentPage, string searchKey = "", int pageSize = 0)
    {
        var path = Path.Combine(_rootPath, relativePath);
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"The directory '{path}' was not found.");
        }

        var assetList = new List<AssetTypes>();

        var searchPattern = string.IsNullOrEmpty(searchKey) ? "*" : $"*{searchKey}*";

        // Add directories to the list
        foreach (var directoryPath in Directory.GetDirectories(path, searchPattern))
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

        int pgSize = (pageSize > 0 ? pageSize : _pageSize);

        int skip = (currentPage - 1) * pgSize;

        

        // Add files to the list sorting it by name ascending
        foreach (var filePath in Directory.GetFiles(path, searchPattern).Skip(skip).Take(pgSize).OrderBy(f => f))
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

    public async Task<bool> UploadFileAsync(byte[] fileContent, string relativeDestinationPath)
    {
        try
        {
            // Determine the absolute path where the file will be saved
            string destinationPath = Path.Combine(_rootPath, relativeDestinationPath);

            // Ensure the destination directory exists
            string destinationDirectory = Path.GetDirectoryName(destinationPath);
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            // Save the file asynchronously
            await File.WriteAllBytesAsync(destinationPath, fileContent);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<AssetTypes> GetFileAsync(string relativeFilePath)
    {
        var path = Path.Combine(_rootPath, relativeFilePath);

        try
        {
            // Get file information
            FileInfo fileInfo = new FileInfo(path);

            // Check if the file exists
            if (!fileInfo.Exists)
            {
                return null;
            }

            // Create and return the AssetTypes object
            return new AssetTypes
            {
                Name = fileInfo.Name,
                IsFolder = false,
                Path = fileInfo.FullName,
                Extension = fileInfo.Extension,
                MimeType = Helpers.GetMimeType(fileInfo.Extension),
                SizeInBytes = (int)fileInfo.Length, // This might need adjustment for large files
                DateCreated = fileInfo.CreationTime,
                DateModified = fileInfo.LastWriteTime
            };
        }
        catch(Exception ex)
        {
            return null;
        }

    }
}
