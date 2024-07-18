using Microsoft.Extensions.Configuration;

namespace JadedAssetManagement.Base;

public interface IFileSystemBase
{
    // Uploads a file to the specified destination.
    Task UploadFileAsync(byte[] fileContent, string relativeFilePath, string destination);

    // Deletes a file from the specified path.
    Task DeleteFileAsync(string relativeFilePath);

    // Deletes a directory from the specified path.
    Task DeleteDirectoryAsync(string relativeDirectoryPath);

    Task<IEnumerable<AssetTypes>> ListFilesAllFiles(string relativePath = "/", string searchKey = "");
    // Lists all files in the specified path or directory.
    Task<IEnumerable<AssetTypes>> ListFilesPaged(string relativePath, int currentPage, string searchKey = "", int pageSize = 0);

    Task<AssetTypes> GetFileAsync(string relativeFilePath);
}