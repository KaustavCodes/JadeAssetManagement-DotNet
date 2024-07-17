using Microsoft.Extensions.Configuration;

namespace JadedAssetManagement.Base;

public interface IFileSystemBase
{
    public int PageSize { get; set; }

    // Uploads a file to the specified destination.
    Task UploadFileAsync(byte[] fileContent, string filePath, string destination);

    // Deletes a file from the specified path.
    Task DeleteFileAsync(string filePath);

    // Deletes a directory from the specified path.
    Task DeleteDirectoryAsync(string filePath);

    Task<IEnumerable<AssetTypes>> ListFilesAllFiles();

    // Lists all files in the specified path or directory.
    Task<IEnumerable<AssetTypes>> ListFilesAsync(int currentPage);

    // Lists all files in the specified path or directory with a specific page size.
    Task<IEnumerable<AssetTypes>> ListFilesAsync(int currentPage, int pgSize);
}