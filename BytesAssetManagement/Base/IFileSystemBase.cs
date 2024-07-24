using Microsoft.Extensions.Configuration;

namespace BytesAssetManagement.Base;

public interface IFileSystemBase
{
    // Uploads a file to the specified destination.
    Task<bool> UploadFileAsync(byte[] fileContent, string relativeDestinationPath);

    // Deletes a file from the specified path.
    Task<bool> DeleteFileAsync(string relativeFilePath);

    // Deletes a directory from the specified path.
    Task<bool> DeleteDirectoryAsync(string relativeDirectoryPath);

    Task<IEnumerable<AssetTypes>> ListFilesAllFiles(string relativePath = "/", string searchKey = "");
    // Lists all files in the specified path or directory.
    Task<IEnumerable<AssetTypes>> ListFilesPaged(string relativePath, int currentPage, string searchKey = "", int pageSize = 0);

    Task<AssetTypes> GetFileAsync(string relativeFilePath);
}