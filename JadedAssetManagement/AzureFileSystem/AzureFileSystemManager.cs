
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using JadedAssetManagement.Base;
using Microsoft.Extensions.Configuration;

namespace JadedAssetManagement.AzureFileSystem;

public class AzureFileSystemManager: IFileSystemBase
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _containerClient;
    private readonly string _containerName = "your-container-name"; // Replace with your container name
    
    public AzureFileSystemManager(IConfiguration configuration)
    {
        string connectionString = configuration["AzureFileSystem:ConnectionString"];
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException("ConnectionString is missing in the configuration");
        }

        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
    }

    public async Task<bool> DeleteDirectoryAsync(string relativeDirectoryPath)
    {
        try
        {
            await foreach (BlobItem blobItem in _containerClient.GetBlobsAsync(prefix: relativeDirectoryPath))
            {
                BlobClient blobClient = _containerClient.GetBlobClient(blobItem.Name);
                await blobClient.DeleteIfExistsAsync();
            }
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception
            return false;
        }
    }

    public async Task<bool> DeleteFileAsync(string relativeFilePath)
    {
        try
        {
            BlobClient blobClient = _containerClient.GetBlobClient(relativeFilePath);
            await blobClient.DeleteIfExistsAsync();
            return true;
        }
        catch (Exception ex)
        {
            // Log exception
            return false;
        }
    }

    public async Task<AssetTypes> GetFileAsync(string relativeFilePath)
    {
        try
        {
            BlobClient blobClient = _containerClient.GetBlobClient(relativeFilePath);
            BlobProperties properties = await blobClient.GetPropertiesAsync();
            var asset = new AssetTypes
            {
                Name = Path.GetFileName(relativeFilePath),
                IsFolder = false,
                Path = relativeFilePath,
                Extension = Path.GetExtension(relativeFilePath),
                MimeType = properties.ContentType,
                SizeInBytes = (int)properties.ContentLength,
                DateCreated = properties.CreatedOn?.DateTime ?? DateTime.MinValue,
                DateModified = properties.LastModified.DateTime
            };
            return asset;
        }
        catch (Exception ex)
        {
            // Handle exceptions
            throw;
        }
    }

    public async Task<IEnumerable<AssetTypes>> ListFilesAllFiles(string relativePath = "/", string searchKey = "")
    {
        var assets = new List<AssetTypes>();
        await foreach (BlobItem blobItem in _containerClient.GetBlobsAsync(prefix: relativePath))
        {
            if (string.IsNullOrEmpty(searchKey) || blobItem.Name.Contains(searchKey))
            {
                string extension = Path.GetExtension(blobItem.Name);
                assets.Add(new AssetTypes
                {
                    Name = Path.GetFileName(blobItem.Name),
                    IsFolder = false,
                    Path = blobItem.Name,
                    Extension = extension,
                    MimeType = Helpers.GetMimeType(extension),
                    SizeInBytes = -1,
                    DateCreated = DateTime.MinValue,
                    DateModified = DateTime.MinValue
                });
            }
        }
        return assets;
    }

    public async Task<IEnumerable<AssetTypes>> ListFilesPaged(string relativePath, int currentPage, string searchKey = "", int pageSize = 0)
    {
        var assets = new List<AssetTypes>();
        int skipCount = currentPage * pageSize;
        await foreach (BlobItem blobItem in _containerClient.GetBlobsAsync(prefix: relativePath))
        {
            if (string.IsNullOrEmpty(searchKey) || blobItem.Name.Contains(searchKey))
            {
                if (skipCount-- > 0) continue;
                if (assets.Count >= pageSize) break;

                string extension = Path.GetExtension(blobItem.Name);
                assets.Add(new AssetTypes
                {
                    Name = Path.GetFileName(blobItem.Name),
                    IsFolder = false,
                    Path = blobItem.Name,
                    Extension = extension,
                    MimeType = Helpers.GetMimeType(extension),
                    SizeInBytes = -1,
                    DateCreated = DateTime.MinValue,
                    DateModified = DateTime.MinValue
                });
            }
        }
        return assets;
    }

    public async Task<bool> UploadFileAsync(byte[] fileContent, string relativeDestinationPath)
    {
        try
        {
            BlobClient blobClient = _containerClient.GetBlobClient(relativeDestinationPath);
            using (var stream = new MemoryStream(fileContent))
            {
                await blobClient.UploadAsync(stream, true);
            }
            return true;
        }
        catch (Exception ex)
        {
            // Log exception
            return false;
        }
    }
}